using System;
using System.Collections.Generic;
using System.Net;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices;
using mPower.Framework;
using ChargifyNET;

namespace mPower.Documents.ExternalServices
{
    public class ChargifyService
    {
        private ChargifyConnect _chargify;
        //I am not sure where i should take it
        private string _organization = "Mpowering";


        private string _chargifyApiPassword = "x";

        public ChargifyService()
        {
        }

        /// <summary>
        /// Connect to chargify before use any method from this service
        /// </summary>
        /// <param name="chargifyUrl">Affiliate chargify url</param>
        /// <param name="sharedKey">Affiliate shared key</param>
        /// <param name="chargifyApiKey">Affiliate chargify api key</param>
        public void Connect(string chargifyUrl, string sharedKey, string chargifyApiKey)
        {
            _chargify = new ChargifyConnect(chargifyUrl,
                chargifyApiKey,
                _chargifyApiPassword,
                sharedKey)
            {
                ProtocolType = SecurityProtocolType.Tls12
            };
        }

        public IDictionary<int, IProduct> GetProducts()
        {
            return _chargify.GetProductList();
        }

        /// <summary>
        /// Create customer on chargify
        /// </summary>
        /// <param name="user">This system user</param>
        /// <returns>created user</returns>
        public ICustomer CreateCustomer(UserDocument user)
        {
            //TODO choose right phone 
            var customer = _chargify.LoadCustomer(user.Id) ??
                           _chargify.CreateCustomer(user.FirstName, user.LastName, user.Email, "", _organization, user.Id);

            return customer;
        }

        /// <summary>
        /// Update existing customer on chargify
        /// </summary>
        /// <param name="user">updated user from this system</param>
        /// <returns>Updated user from chargify</returns>
        public ICustomer UpdateCustomer(UserDocument user)
        {
            var customer = new Customer(user.FirstName, user.LastName, user.Email, _organization, user.Id);

            return _chargify.UpdateCustomer(customer);
        }

        /// <summary>
        /// Get customer info from chargify
        /// </summary>
        /// <param name="userId">This system user id</param>
        /// <returns>Customer info</returns>
        public ICustomer GetCustomer(string userId)
        {
            return _chargify.LoadCustomer(userId);
        }

        /// <summary>
        /// Subscribe user to product with specified from chargify productHandle 
        /// </summary>
        /// <param name="productHandle">Product identifier</param>
        /// <param name="customerId">This system user Id</param>
        /// <param name="attributes">Credit Card information</param>
        /// <returns>null </returns>
        public ISubscription SubscribeUser(string productHandle,
                                           string customerId,
                                           ICreditCardAttributes attributes)
        {
            return _chargify.CreateSubscription(productHandle, customerId, attributes);
        }

        /// <summary>
        /// Cancel Subscription
        /// </summary>
        public bool CancelSubscription(int subscriptionId, string cancelMessage)
        {
            return _chargify.DeleteSubscription(subscriptionId, cancelMessage);
        }

        /// <summary>
        /// Load Subscription by id
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public ISubscription GetSubscription(int subscriptionId)
        {
            return _chargify.LoadSubscription(subscriptionId);
        }

        public string GetUrlToHostedSignupPage(int productId, string customerId)
        {
            return String.Format("{0}/h/{1}/subscriptions/new?reference={2}", _chargify.URL, productId, customerId);
        }
    }
}
