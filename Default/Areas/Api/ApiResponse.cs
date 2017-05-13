using System;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Default.Areas.Api
{
    public class ApiResponse
    {
        private readonly Controller _controller;
        private ApiResponseStatusEnum _status = ApiResponseStatusEnum.Success;
        private string _data;
        private int _errorCode = 0;

        public ApiResponse(Controller controller)
        {
            _controller = controller;
        }

        public object ToJsonObject()
        {
            var log = GetLog();

            return new ApiResponseObject
            {
                status = _status,
                data = _data,
                log = log,
                error_code = _errorCode
            };
        }

        public void SetErrorCode(int code)
        {
            _errorCode = code;
            _status = ApiResponseStatusEnum.Error;
        }

        public void AddData(object jsonObject)
        {
            _data = JsonConvert.SerializeObject(jsonObject);
        }

        /// <summary>
        /// Get model errors log only in case if we are not set errorCode before
        /// </summary>
        private string GetLog()
        {
            var sb = new StringBuilder();

            if (!_controller.ModelState.IsValid)
            {
                // in case if error code wasn't set just change it's value to 1 (model state errors)
                if (_errorCode == 0) _errorCode = 1;
                _status = ApiResponseStatusEnum.Error;

                foreach (var key in _controller.ModelState.Keys)
                {
                    var errors = _controller.ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        sb.AppendFormat("{0}: {1}{2}", key, error.ErrorMessage, Environment.NewLine);
                    }
                }
            }

            return sb.ToString();
        }
    }
}