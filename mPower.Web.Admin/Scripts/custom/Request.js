/**
* Holds all request objects on page
*/
var RequestRegistry = function () {
    RequestRegistry.requests = [];
}

/**
* Request name contants
*/
RequestRegistry.DEFAULT_REQUEST = "REQUEST_DEFAULT";

RequestRegistry = {
    /**
    * Hash of Request object by Request name
    */
    requests: {},

    /**
    * Register request
    */
    addRequest: function (request) {
        this.requests[request.getName()] = request;
    },

    /**
    * Returns request if it exist, or create it
    */
    insure: function (name) {
        if (!this.requests.hasOwnProperty(name)) {
            var request = new Request();
            request.setName(name);
            this.requests[name] = request;
        }

        return this.requests[name];
    }
};

/**
* Request object
*/
var Request = function Request() {
    this.params = new RequestParamCollection();
    this.successFunctions = [];
    this.onErrorFunctions = [];
};

/**
* Factory method for creating or accessing to named Request
*/
Request.named = function (name) {
    return RequestRegistry.insure(name);
};

/**
* Build Request object for submitting form
*/
Request.form = function (formId) {
    var request = new Request();
    request.setFormId(formId);
    return request;
};

/**
* Build Request object for GET request
*/
Request.get = function (url) {
    var request = new Request();
    request.setMethod("GET");
    request.setUrl(url);
    return request;
};

/**
* Build Request object for POST request
*/
Request.post = function (url) {
    var request = new Request();
    request.setMethod("POST");
    request.setUrl(url);
    return request;
};

Request.prototype = {
    /**
    * Name of request
    */
    name: null,

    /**
    * Url
    */
    url: null,

    /**
    * Hash of functions by unique key. Called on success
    */
    successFunctions: {},

    /**
    * Hash of functions by unique key. Called on error
    */
    onErrorFunctions: {},

    /**
    * HTTP Method
    */
    method: "POST",

    /**
    * Asynchronous or not. Boolean.
    */
    async: true,

    /**
    * Should we show block UI?
    */
    _blockUI: false,

    /**
    * RequestParamCollection 
    */
    params: null,

    /**
    * Request raw data (used instead of 'params')
    */
    rawData: null,

    /**
    * HTML Form Id to be submitted by Ajax
    * It is possible to submit only one form per request
    */
    formId: null,

    /**
    * TODO: we cannot change name, when request already registered in RequestRegistry - not implemented now.
    */
    setName: function (name) {
        this.name = name;
    },

    /**
    * Name of the request
    */
    getName: function () {
        return this.name;
    },

    /**
    * Set HTTP method
    */
    setMethod: function (method) {
        this.method = method;
        return this;
    },

    /**
    * Set form id
    */
    setFormId: function (formId) {
        this.formId = formId;
        return this;
    },

    /**
    * Get form id
    */
    getFormId: function () {
        return this.formId;
    },

    /**
    * Get action url
    */
    getUrl: function () {
        return this.url;
    },

    /**
    * Set action url
    */
    setUrl: function (url) {
        this.url = url;
        return this;
    },

    /**
    * Get async status
    */
    getAsync: function () {
        return this.async;
    },

    /**
    * Set async status
    */
    setAsync: function (async) {
        this.async = async;
        return this;
    },

    /**
    * Add params from associative object
    */
    addParams: function (object) {
        this.params.addParams(object);
        return this;
    },

    /**
    * Clear params
    */
    clearParams: function () {
        this.params.clear();
        return this;
    },

    /**
    * Set raw data
    */
    setJSON: function (data) {
        this.rawData = data;
        return this;
    },

    /**
    * Add function which will be called on success
    */
    addSuccess: function (key, func) {
        this.successFunctions[key] = func;
        return this;
    },

    /**
    * Add function which will be called on error
    */
    addOnError: function (key, func) {
        this.onErrorFunctions[key] = func;
        return this;
    },

    /**
    * Should we block UI or not.
    */
    blockUI: function (flag) {
        this._blockUI = flag;
        return this;
    },

    /**
    * Block UI
    */
    doBlockUI: function () {
        $("#ajaxLoader").show();
        if (this._blockUI != true)
            return;

        if (!$.blockUI)
            return;

        $('#cboxOverlay').css('background', 'none');
        $.blockUI(this.blockUIOptions);

    },

    /**
    * Unblock UI
    */
    doUnblockUI: function () {
        $("#ajaxLoader").hide();
        $('#cboxOverlay').css('background');

        if (!$.unblockUI)
            return;

        $.unblockUI(this.blockUIOptions);

    },

    /**
    * Sending request
    */
    send: function () {
        // block UI
        this.doBlockUI();

        if (this.formId != null) {

            // Submit for using jQuery Form Plugin
            $('#' + this.formId).ajaxSubmit({
                type: this.method,
                cache: false,
                dataType: 'json',
                success: Function.reference(this, this.success),
                error: Function.reference(this, this.error),
                beforeSubmit: Function.reference(this, this.beforeSend)
            });
        } else {

            // Submit data via 'plain' jQuery ajax
            $.ajax({
                url: this.url,
                cache: false,
                type: this.method,
                data: this.rawData != null ? JSON.stringify(this.rawData) : this.params.toObject(),
                contentType: this.rawData != null ? 'application/json' : 'application/x-www-form-urlencoded',
                dataType: 'json',
                success: Function.reference(this, this.success),
                error: Function.reference(this, this.error),
                async: this.getAsync()
            });
        }
    },

    /**
    * On success event
    */
    success: function (data) {
        // unblock UI
        this.doUnblockUI();


        // getting result from untyped json object to typed Result object
        var result = Result.fromJson(data);
        var options = result.options;

        if (result.getClosePopup()) {
            $.fn.colorbox.close();
        }

        if (result.getReloadPage()) {
            window.location.reload();
        }

        // if errors found - show error and stop the flow
        if (result.hasErrors()) {
            result.getErrors().show(result.options);

            // Call onError functions
            for (var i in this.onErrorFunctions) {
                var func = this.onErrorFunctions[i];
                func(result);
            }

            if (options.successSummaryContainer != null) {
                $('#' + options.successSummaryContainer).html('');
                $('#' + options.successSummaryContainer).parent().hide();
            }

            return;
        }

        // if redirect url specified - do redirect and stop flow
        if (result.getRedirectUrl()) {
            var url = result.getRedirectUrl();
            window.location.href = url;
            return;
        }

        if (result.hasUpdateItems()) {
            result.getUpdateItems().update();
        }

        result.getErrors().clear(options);
        if (options.successSummaryContainer != null && options.successMessage != null) {
            $('#' + options.successSummaryContainer).html('');
            $('#' + options.successSummaryContainer).parent().show();
            $('#' + options.successSummaryContainer).append('<li>' + options.successMessage + ' </li>');
            //            setTimeout(function() {
            //                $('#' + options.successSummaryContainer).parent().hide();
            //            }, 10000);
        }

        // Call success functions
        for (var i in this.successFunctions) {
            var func = this.successFunctions[i];
            func(result);
        }
    },

    /**
    * Before send event
    */
    beforeSend: function (params) {

    },

    /**
    * On error event
    */
    error: function (arg) {
        // block UI
        this.doUnblockUI();
    },

    get: function (url) {
        this.setMethod("GET");
        this.setUrl(url);
        return this;
    },

    post: function (url) {
        this.setMethod("POST");
        this.setUrl(url);
        return this;
    },

    form: function (formId) {
        this.setFormId(formId);
        return this;
    }
}


/**
* Request param
*/
var RequestParam = function (name, value) {
    this.name = name;
    this.value = value;
}

RequestParam.prototype = {
    getName: function () {
        return this.name;
    },

    getValue: function () {
        return this.value;
    }
}


/**
* Params collection
*/
var RequestParamCollection = function () {
    this.params = [];
}

RequestParamCollection.prototype = {
    /**
    * Array of RequestParams
    */
    params: [],

    /**
    * Add param to collection
    */
    addParam: function (name, value) {
        var newParam = new RequestParam(name, value);

        // if param with such name already added - replace it with new RequestParam (i.e. overwrite)
        for (var i = 0; i < this.params.length; i++) {
            var param = this.params[i];
            if (param.getName() == name) {
                this.params[i] = newParam;
                return;
            }
        }

        // add new param, if not exist such
        this.params.push(newParam);
    },

    /**
    * Add params from associative object
    */
    addParams: function (object) {
        for (var name in object) {
            var value = object[name];
            this.addParam(name, value);
        }
    },

    /**
    * Clear params
    */
    clear: function () {
        this.params = [];
    },

    /**
    * To associative object
    */
    toObject: function () {
        var object = {};
        for (var i = 0; i < this.params.length; i++) {
            var param = this.params[i];
            object[param.name] = param.value;
        }
        return object;
    },

    /**
    * To request string
    */
    toString: function () {
        var string = '';
        for (var i = 0; i < this.params.length; i++) {
            var prefix = i == 0 ? '?' : '&';
            var param = this.params[i];
            string += prefix + escape(param.name) + '=' + escape(param.value);
        }
        return string;
    }
}

