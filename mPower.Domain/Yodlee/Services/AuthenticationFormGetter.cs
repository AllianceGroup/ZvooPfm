using System.Collections.Generic;
using System.Linq;
using com.yodlee.core.dataservice;
using com.yodlee.core.mfarefresh;
using com.yodlee.soap.core.accountmanagement;
using com.yodlee.soap.core.refresh;
using mPower.Domain.Yodlee.Form;

namespace mPower.Domain.Yodlee.Services
{
    public class AuthenticationFormGetter : BaseYodleeService
    {
        
        public List<FormInputGroup> GetAuthenticationForm(string username, string password, int contentServiceId)
        {
            ConnectToYodlee();
            var user = LoginUser(username, password);

            var itemManagementService = new ItemManagementService();

            var authenticationForm = itemManagementService.getLoginFormForContentService(user.userContext, contentServiceId);

            if (authenticationForm == null)
                return null;

            var formInputGroups = new List<FormInputGroup>();
            FormInputGroup formInputGroup = null;
            // FormHelpers is a recursive call that loops  through the form input object and create a FormInputGroup Object
            FormHelpers.BuildComponents(ref formInputGroups, authenticationForm.componentList, ref formInputGroup);

            return formInputGroups.ToList();
        }

        public List<FormInputGroup> GetReAuthenticationForm(string username, string password,
                                                                   int contentServiceItemId)
        {
            ConnectToYodlee();
            var user = LoginUser(username, password);

            var itemManagementService = new ItemManagementService();

            var authenticationForm = itemManagementService.getLoginFormCredentialsForItem(user.userContext,
                                                                                           contentServiceItemId);

            if (authenticationForm == null)
                return null;

            var formInputGroups = new List<FormInputGroup>();
            FormInputGroup formInputGroup = null;
            // FormHelpers is a recursive call that loops  through the form input object and create a FormInputGroup Object
            FormHelpers.BuildComponents(ref formInputGroups, authenticationForm.componentList, ref formInputGroup);

            return formInputGroups.ToList();
        }
        public FormInputGroup GetMfaAuthenticationForm(string loginName, string password, long contentServiceItemId)
        {
            ConnectToYodlee();
            var user = LoginUser(loginName, password);
            var refreshService = new RefreshService();
            try
            {
                var mfaRefreshStatus = refreshService.getMFAResponse(user.userContext, contentServiceItemId);
                var securityQuestionFieldInfo = mfaRefreshStatus.fieldInfo as SecurityQuestionFieldInfo;

                if (securityQuestionFieldInfo != null && securityQuestionFieldInfo.questionAndAnswerValues.Length > 0)
                    return FormHelpers.BuildQuestionAnswers(securityQuestionFieldInfo.questionAndAnswerValues);

                if (mfaRefreshStatus.errorCode == GathererErrors.STATUS_OK)
                    return null;

            }
            catch
            {

            }

            return null;
        }


        #region Old Code from Content Service Item Mfa Authentication Form
        //public List<FormInputGroup> GetMfaAuthenticationForm(CobrandContext cobrandContext, long contentServiceItemId)
        //{
        //    var authenticationForm = _itemManagementService.getMfaLoginFormForItem(_userContext, contentServiceItemId);
        //    var questions = _itemManagementService.getMfaQuestionsAndAnswersForItem(_userContext, contentServiceItemId);

        //    if (authenticationForm == null)
        //        return null;

        //    var formInputGroups = new List<FormInputGroup>();
        //    FormInputGroup formInputGroup = null;
        //    // FormHelpers is a recursive call that loops  through the form input object and create a FormInputGroup Object
        //    FormHelpers.BuildComponents(ref formInputGroups, authenticationForm.componentList, ref formInputGroup);

        //    return formInputGroups.ToList();

        //}

        //public long SubmitMfaAuthenticationForm(Dictionary<string, object> parameters, int contentServiceItemId)
        //{
        //    var authenticationForm = _itemManagementService.getMfaLoginFormForItem(_userContext, contentServiceItemId);

        //    //Fills out the yodlee form
        //    FormHelpers.BuildParameterList(ref authenticationForm.componentList, parameters);

        //    _itemManagementService.updateCredentialsForItem(_userContext, contentServiceItemId,
        //                                                 authenticationForm.componentList);
        //    // Starts Yodlee Refreshing Data
        //    var refreshService = new RefreshService();
        //    var refreshStatus = refreshService.startRefresh(_userContext, contentServiceItemId, 1);

        //    long[] failedStatus = { 0, 3, 7, 5 };

        //    return refreshStatus.status;
        //}






        #endregion
    }
}