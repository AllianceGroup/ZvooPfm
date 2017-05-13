using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.Web.Administration;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.ExternalServices;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Framework.Utils;
using mPower.Web.Admin.Models.Affiliate;

namespace mPower.Web.Admin.Controllers
{
    public class AffiliatesController : BaseAdminController
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly ChargifyService _chargifyService;



        public AffiliatesController(AffiliateDocumentService affiliateService,
                                   ChargifyService chargifyService)
        {
            _affiliateService = affiliateService;
            _chargifyService = chargifyService;
        }

        public ActionResult Index()
        {
            var model = new AffiliateModel();
            model.Affiliates = _affiliateService.GetAll().Select(x => new SelectListItem() { Text = x.ApplicationName, Value = x.ApplicationId }).ToList();
            model.IisAppPoolName = "mPower";

            return View(model);
        }

        public ActionResult Edit(string id)
        {
            var affiliate = _affiliateService.GetById(id);
            var products = affiliate.Products;
            var selectList = new List<SelectListItem>() { new SelectListItem() { Text = "Please, select.", Value = "0" } };
            selectList.AddRange(products.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString()}));

            var model = new UpdateAffiliateModel
            {
                ApplicationName = affiliate.ApplicationName,
                ApplicationId = affiliate.ApplicationId,
                ChargifyApiKey = affiliate.ChargifyApiKey,
                ChargifySharedKey = affiliate.ChargifySharedKey,
                ChargifyUrl = affiliate.ChargifyUrl,
                ContactPhoneNumber = affiliate.ContactPhoneNumber,
                DisplayName = affiliate.DisplayName,
                EmailSuffix = affiliate.EmailSuffix,
                LegalName = affiliate.LegalName,
                MembershipApiKey = affiliate.MembershipApiKey,
                SmtpCredentialsEmail = affiliate.Smtp.CredentialsEmail,
                SmtpCredentialsUserName = affiliate.Smtp.CredentialsUserName,
                SmtpCredentialsPassword = affiliate.Smtp.CredentialsPassword,
                SmtpEnableSsl = affiliate.Smtp.EnableSsl,
                SmtpHost = affiliate.Smtp.Host,
                SmtpPort = affiliate.Smtp.Port,
                UrlPaths = affiliate.UrlPaths == null ? String.Empty : String.Join(",", affiliate.UrlPaths),
                AssemblyName = affiliate.AssemblyName,
                JanrainAppApiKey = affiliate.JanrainAppApiKey,
                JanrainAppUrl = affiliate.JanrainAppUrl,
                PfmEnabled = affiliate.PfmEnabled,
                BfmEnabled = affiliate.BfmEnabled,
                CreditAppEnabled = affiliate.CreditAppEnabled,
                SavingsEnabled = affiliate.SavingsEnabled,
                MarketingEnabled = affiliate.MarketingEnabled,
                Address = affiliate.Address,
                Products = selectList,
                Product = affiliate.SignupProductId,
                ProductWithoutTrial = affiliate.SignupProductIdWithoutTrial,
                ProductCreditIdentity = affiliate.AdditionalCreditIdentityProduct,
                PublicUrl = affiliate.PublicUrl,
            };

            return View(model);
        }

        public ActionResult Create([Bind(Prefix = "CreateModel")]CreateAffiliateModel model)
        {
            if (_affiliateService.GetById(model.AffiliateId) != null)
                ModelState.AddModelError("CreateModel.AffiliateId", "Affiliate with such app id already exists");

            if (ModelState.IsValid)
            {
                var command = new Affiliate_CreateCommand { Id = model.AffiliateId, Name = model.AffiliateName };

                Send(command);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Update(UpdateAffiliateModel model)
        {
            var command = new Affiliate_UpdateCommand
            {
                ApplicationId = model.ApplicationId,
                ApplicationName = model.ApplicationName,
                ChargifyApiKey = model.ChargifyApiKey,
                ChargifySharedKey = model.ChargifySharedKey,
                ChargifyUrl = model.ChargifyUrl,
                ContactPhoneNumber = model.ContactPhoneNumber,
                DisplayName = model.DisplayName,
                EmailSuffix = model.EmailSuffix,
                LegalName = model.LegalName,
                MembershipApiKey = model.MembershipApiKey,
                SmtpCredentialsEmail = model.SmtpCredentialsEmail,
                SmtpCredentialsUserName = model.SmtpCredentialsUserName,
                SmtpCredentialsPassword = model.SmtpCredentialsPassword,
                SmtpEnableSsl = model.SmtpEnableSsl,
                SmtpHost = model.SmtpHost,
                SmtpPort = model.SmtpPort,
                UrlPaths = String.IsNullOrEmpty(model.UrlPaths)
                                ? new List<string>()
                                : model.UrlPaths.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                AssemblyName = model.AssemblyName,
                JanrainAppApiKey = model.JanrainAppApiKey,
                JanrainAppUrl = model.JanrainAppUrl,
                PfmEnabled = model.PfmEnabled,
                BfmEnabled = model.BfmEnabled,
                CreditAppEnabled = model.CreditAppEnabled,
                SavingsEnabled = model.SavingsEnabled,
                MarketingEnabled = model.MarketingEnabled,
                Address = model.Address,
                SignUpProductId = model.Product,
                SignupProductIdWithTrial = model.ProductWithoutTrial,
                AdditionalCreditIdentityProduct = model.ProductCreditIdentity,
                PublicUrl = model.PublicUrl,
            };

            Send(command);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            var command = new Affiliate_DeleteCommand() { Id = id };

            Send(command);

            return RedirectToAction("Index");
        }

        public ActionResult RestartSite(string iisAppPoolName)
        {
            var server = new ServerManager();
            var pool = server.ApplicationPools.FirstOrDefault(x => x.Name.Equals(iisAppPoolName));
            if (pool != null)
            {
                if (pool.State == ObjectState.Started)
                {
                    //stop the site...
                    if (pool.State != ObjectState.Stopped)
                    {
                        pool.Stop();
                    }
                    WaitForState(pool, ObjectState.Stopped, 30);

                    if (pool.State == ObjectState.Stopped)
                    {
                        //do deployment tasks...
                    }
                    else
                    {
                        throw new InvalidOperationException("Could not stop website!");
                    }

                    //restart the site...
                    pool.Start();
                    WaitForState(pool, ObjectState.Started, 30);
                    if (pool.State != ObjectState.Started)
                    {
                        throw new InvalidOperationException("Could not start website!");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Could not find website!");
            }

            return RedirectToAction("Index");
        }

        public ActionResult SynchProducts(string AffiliateId)
        {
            SyncChargifyProducts(AffiliateId);

            AjaxResponse.RedirectUrl = Url.Action("Edit", new {id = AffiliateId});

            return Result();
            
        }

        internal void SyncChargifyProducts(string AffiliateId)
        {
            var affiliate = _affiliateService.GetById(AffiliateId);

            _chargifyService.Connect(affiliate.ChargifyUrl, affiliate.ChargifySharedKey, affiliate.ChargifyApiKey);


            var products = _chargifyService.GetProducts();

            var result = new List<ChargifyProductData>();
            foreach (var item in products)
            {
                var product = item.Value;
                var productId = item.Key;
                
                var productData = new ChargifyProductData
                                      {
                                          AccountingCode = product.AccountingCode,
                                          Description = product.Description,
                                          Handle = product.Handle,
                                          Id = productId,
                                          Name = product.Name,
                                          PriceInCents = product.PriceInCents,
                                          SignUpPage = product.PublicSignupPages.FirstOrDefault().URL
                                      };

                result.Add(productData);
            }

            var existingProducts = affiliate.Products ?? new List<ChargifyProductDocument>();
            foreach (var item in existingProducts)
            {
                var resultProduct = result.FirstOrDefault(x => x.Id == item.Id);

                //this will be only in case if project was archived, because of chargify not sending archived products
                //but we need it
                if (resultProduct == null)
                {
                    result.Add(new ChargifyProductData()
                                   {
                                       AccountingCode = item.AccountingCode,
                                       Description = item.Description,
                                       Handle = item.Handle,
                                       Id = item.Id,
                                       Name = item.Name,
                                       PriceInCents = item.PriceInCents,
                                       IsArchived = true,
                                       SignUpPage = item.SignUpPage
                                   });
                }
            }

            var command = new Affiliate_SynchronizeChargifyProductsCommand()
                              {
                                  AffiliateId = affiliate.ApplicationId,
                                  Products = result
                              };

            Send(command);
        }

        public ActionResult GetUniqueKey()
        {
            var key = SecurityUtil.GetUniqueToken();

            return new JsonResult() { Data = key, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private void WaitForState(ApplicationPool pool, ObjectState state, int timeoutInSecs)
        {
            var secondsPassed = 0;
            while (pool.State != state)
            {
                Thread.Sleep(1000);
                if (++secondsPassed > timeoutInSecs)
                {
                    return;
                }
            }
        }
    }
}
