﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using umbraco;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Payment.SagePay
{
	public class SagePayPaymentProvider : SagePayPaymentBase, IPaymentProvider
	{
	    public PaymentTransactionMethod GetParameterRenderMethod()
		{
			return PaymentTransactionMethod.Inline;
		}

		public IEnumerable<PaymentProviderMethod> GetAllPaymentMethods(int id)
		{
            var paymentProvider = PaymentProvider.GetPaymentProvider(id);

            var providerSettingsXML = paymentProvider.GetSettingsXML();

            var enabledServices = providerSettingsXML.Descendants("Services").FirstOrDefault();

            var paymentMethods = new List<PaymentProviderMethod>();

			if (enabledServices != null)
				foreach (var service in enabledServices.Descendants("Service"))
				{
					var serviceName = service.Attribute("name").Value;
					var serviceTitle = serviceName;

					if (service.Attribute("title") != null)
					{
						serviceTitle = service.Attribute("title").Value;
					}

					var paymentImageId = 0;

					if (service.Descendants().Any())
					{
						foreach (var issuer in service.Descendants("Issuer"))
						{
							var name = issuer.Attribute("name").Value;
							var code = issuer.Attribute("code").Value;

							var method = code;

							var nameForLogoDictionaryItem = string.Format("{0}LogoId", code.Replace(" ", string.Empty));

							var logoDictionaryItem = library.GetDictionaryItem(nameForLogoDictionaryItem);

							if (string.IsNullOrEmpty(logoDictionaryItem))
							{
								int.TryParse(library.GetDictionaryItem(nameForLogoDictionaryItem), out paymentImageId);
							}

							paymentMethods.Add(new PaymentProviderMethod
							{
								Id = method,
								Description = name,
								Title = name,
								Name = serviceTitle,
								ProviderName = GetName(),
								ImageId = paymentImageId
							});
						}
					}
					else
					{
						var nameForLogoDictionaryItem = string.Format("{0}LogoId", serviceName.Replace(" ", string.Empty));

						var logoDictionaryItem = library.GetDictionaryItem(nameForLogoDictionaryItem);

						if (string.IsNullOrEmpty(logoDictionaryItem))
						{
							int.TryParse(library.GetDictionaryItem(nameForLogoDictionaryItem), out paymentImageId);
						}

						paymentMethods.Add(new PaymentProviderMethod
						{
							Id = serviceName,
							Description = serviceName,
							Title = serviceTitle,
							Name = serviceName,
							ProviderName = GetName(),
							ImageId = paymentImageId
						});
					}
				}


			return paymentMethods;
		}
	}
}