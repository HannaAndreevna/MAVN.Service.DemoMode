using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerManagement.Client.Models.Requests;
using Lykke.Service.CustomerManagement.Contract.Events;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Requests;
using Lykke.Service.DemoMode.Domain.Entities;
using Lykke.Service.DemoMode.Domain.Services;
using Lykke.Service.PartnersIntegration.Client;
using Lykke.Service.PartnersIntegration.Client.Enums;
using Lykke.Service.PartnersIntegration.Client.Models;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.PartnerManagement.Client.Models;
using Lykke.Service.PartnerManagement.Client.Models.Partner;
using Lykke.Service.PartnersPayments.Contract;
using Lykke.Service.Sessions.Contracts;

namespace Lykke.Service.DemoMode.DomainServices.Services
{
    public class DemoModeService : IDemoModeService
    {
        private readonly IRabbitPublisher<EmailCodeVerifiedEvent> _emailCodeVerifiedPublisher;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IPartnersIntegrationClient _partnersIntegrationClient;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly PaymentRequestModelSettings _paymentRequestSettings;
        private readonly ICustomerManagementServiceClient _customerManagementServiceClient;
        private readonly string _demoModeEmailSuffix;
        private readonly ILog _log;
        private readonly Money18 _tokensAmount;

        public DemoModeService(
            IRabbitPublisher<EmailCodeVerifiedEvent> emailCodeVerifiedPublisher,
            ICustomerProfileClient customerProfileClient,
            IPartnersIntegrationClient partnersIntegrationClient,
            IPartnerManagementClient partnerManagementClient,
            PaymentRequestModelSettings paymentRequestSettings,
            ICustomerManagementServiceClient customerManagementServiceClient,
            string demoModeEmailSuffix,
            ILogFactory logFactory)
        {
            if (paymentRequestSettings.FiatAmount.HasValue && !string.IsNullOrWhiteSpace(paymentRequestSettings.TokensAmount))
            {
                throw new ArgumentException("Cannot have both FiatAmount and TokensAmount set");
            }
            if (!paymentRequestSettings.FiatAmount.HasValue && string.IsNullOrWhiteSpace(paymentRequestSettings.TokensAmount))
            {
                throw new ArgumentException("FiatAmount or TokensAmount required");
            }

            _emailCodeVerifiedPublisher = emailCodeVerifiedPublisher;
            _customerProfileClient = customerProfileClient;
            _partnersIntegrationClient = partnersIntegrationClient;
            _partnerManagementClient = partnerManagementClient;
            _paymentRequestSettings = paymentRequestSettings;
            _customerManagementServiceClient = customerManagementServiceClient;
            _demoModeEmailSuffix = demoModeEmailSuffix;
            _log = logFactory.CreateLog(this);

            if (!string.IsNullOrWhiteSpace(paymentRequestSettings.TokensAmount))
            {
                var tokensAmountCorrect = Money18.TryParse(paymentRequestSettings.TokensAmount, out _tokensAmount);

                if (!tokensAmountCorrect)
                {
                    throw new ArgumentException("Invalid value for TokensAmount", nameof(paymentRequestSettings.TokensAmount));
                }
            }
        }

        public async Task ProcessCustomerRegistrationAsync(CustomerRegistrationEvent message)
        {
            var emailAndDemoMode = await IsDemoMode(message.CustomerId);

            if (!emailAndDemoMode.Item2)
            {
                _log.Info("Customer email not in demo mode", new {message.CustomerId});
                return;
            }

            _log.Info("Starting to process customer registration event", new
            {
                message.CustomerId,
                message.ReferralCode,
                message.TimeStamp,
                emailAndDemoMode.Item1
            });

            var phoneNumber = string.Join("", Guid.NewGuid().ToString().ToCharArray().Where(char.IsDigit).Take(15));
            _log.Info("Setting customer's phone", new
            {
                message.CustomerId,
                phoneNumber
            });

            var setPhoneRequest = new SetCustomerPhoneInfoRequestModel
            {
                CustomerId = message.CustomerId,
                CountryPhoneCodeId = 1,
                PhoneNumber = phoneNumber
            };
            var phoneResult = await _customerProfileClient.CustomerPhones.SetCustomerPhoneInfoAsync(setPhoneRequest);
            if (phoneResult.ErrorCode != CustomerProfileErrorCodes.None)
            {
                _log.Error(message: $"Couldn't set customer phone - {phoneResult.ErrorCode}", context: setPhoneRequest);
                return;
            }

            _log.Info("Auto verifying customer phone", new
            {
                message.CustomerId,
            });
            var verifiedPhoneResult = await _customerProfileClient.CustomerPhones.SetCustomerPhoneAsVerifiedAsync(new SetPhoneAsVerifiedRequestModel
            {
                CustomerId = message.CustomerId
            });
            if (verifiedPhoneResult.ErrorCode != CustomerProfileErrorCodes.None)
            {
                _log.Error(message: $"Couldn't verify phone number - {verifiedPhoneResult.ErrorCode}", context: message.CustomerId);
                return;
            }

            _log.Info("Auto verifying customer email", new
            {
                message.CustomerId,
            });
            await _emailCodeVerifiedPublisher.PublishAsync(new EmailCodeVerifiedEvent
            {
                CustomerId = message.CustomerId, TimeStamp = DateTime.UtcNow
            });

            //This is actually expected to run later,
            //so we don't want to block the current thread and let the next message be processed
#pragma warning disable 4014
            Task.Run(async () => {
#pragma warning restore 4014
                await Task.Delay(_paymentRequestSettings.PaymentRequestDelayInMilliseconds);

                await CreatePartnerPaymentRequestAsync(message.CustomerId);
            });
        }

        public async Task ProcessSessionEndedAsync(SessionEndedEvent message)
        {
            var emailAndDemoMode = await IsDemoMode(message.CustomerId);

            if (!emailAndDemoMode.Item2)
            {
                _log.Info("Customer email not in demo mode", new { message.CustomerId });
                return;
            }

            var newEmail = $"{Guid.NewGuid()}@{_demoModeEmailSuffix}";

            _log.Info("Changing user's email because of logout", new
            {
                message.CustomerId,
                message.TimeStamp,
                message.Token,
                oldEmail = emailAndDemoMode.Item1,
                newEmail
            });

            await _customerManagementServiceClient.EmailsApi.UpdateEmailAsync(new UpdateEmailRequestModel
            {
                CustomerId = message.CustomerId,
                NewEmail = newEmail
            });
        }
        
        public async Task ProcessPartnersPaymentTokensReservedAsync(PartnersPaymentTokensReservedEvent message)
        {
            var emailAndDemoMode = await IsDemoMode(message.CustomerId);

            if (!emailAndDemoMode.Item2)
            {
                _log.Info("Customer email not in demo mode", new { message.CustomerId });
                return;
            }

            _log.Info("Automatically executing payment", new
            {
                message.CustomerId,
                message.PaymentRequestId,
                message.PartnerId,
                message.LocationId,
                message.Timestamp
            });

            await _partnersIntegrationClient.PaymentsApi.ExecutePaymentRequestAsync(new PaymentsExecuteRequestModel
            {
                PaymentRequestId = message.PaymentRequestId,
                PartnerId = message.PartnerId
            });
        }

        private async Task<Tuple<string, bool>> IsDemoMode(string customerId)
        {
            var customer = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(customerId, true);

            if (customer.ErrorCode != CustomerProfileErrorCodes.None)
            {
                _log.Error(null, "Error retrieving customer profile", new { customerId });
                return new Tuple<string, bool>(string.Empty, false);
            }

            return new Tuple<string, bool>(customer.Profile.Email,
                customer.Profile.Email.EndsWith("@" + _demoModeEmailSuffix,
                    StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task CreatePartnerPaymentRequestAsync(string customerId)
        {
            _log.Info("Sending payment request to customer", new { customerId });

            var partnerRes = await _partnerManagementClient.Partners.GetAsync(
                new PartnerListRequestModel
                {
                    CurrentPage = 1,
                    PageSize = 1,
                    Vertical = Vertical.Hospitality
                });
            var partnerId = partnerRes.PartnersDetails.First().Id;
            var location = await _partnerManagementClient.Partners.GetByIdAsync(partnerId);
            var externalLocationId = location.Locations.First().ExternalId;

            var request = new PaymentsCreateRequestModel
            {
                CustomerId = customerId,
                Currency = _paymentRequestSettings.Currency,
                PartnerId = partnerId.ToString(),
                ExternalLocationId = externalLocationId,
                //PosId = _paymentRequestSettings.PosId,
                ExpirationTimeoutInSeconds = _paymentRequestSettings.CustomerExpirationInSeconds,
                PaymentInfo = _paymentRequestSettings.PaymentInfo,
                PaymentProcessedCallbackUrl = null,
                RequestAuthToken = null,
                TotalFiatAmount = _paymentRequestSettings.TotalFiatAmount
            };

            if (_paymentRequestSettings.FiatAmount.HasValue)
            {
                request.FiatAmount = _paymentRequestSettings.FiatAmount;
            }
            else if (!string.IsNullOrWhiteSpace(_paymentRequestSettings.TokensAmount))
            {
                request.TokensAmount = _tokensAmount;
            }

            var paymentRequestResult = await _partnersIntegrationClient.PaymentsApi.CreatePaymentRequestAsync(request);
            if (paymentRequestResult.Status != PaymentCreateStatus.OK)
                _log.Error(message: $"Couldn't create payment request - {paymentRequestResult.Status}", context: request);
        }
    }
}
