
Function.reference = function(context, func) {
    return function() {
        func.apply(context, arguments);
    };
};

/**
* Static function for counting size of associative object (hash)
*/
Object.size = function(obj) {
    var size = 0, key;
    for (key in obj) {
        if (obj.hasOwnProperty(key)) size++;
    }
    return size;
};

/**
* Checks if input is a string 
*/
Object.isString = function(input) {
    return typeof (input) == 'string';
};


/**
* Single error message 
**/
var Error = function(name, message) {
    this.name = name;
    this.message = message;
};

Error.prototype = {
    /**
    * Error name
    */
    name: null,

    /**
    * Text message
    */
    message: null,

    /**
    * Error name
    */
    getName: function() {
        return this.name;
    },

    /**
    * Error message
    */
    getMessage: function() {
        return this.message;
    }
};


/**
* Errors collection
*/
var Errors = function() {
    this.errors = { };
};

Errors.prototype = {
    /**
    * Error container name attribute
    */
    ERROR_NAME_ATTRIBUTE: "error_container",

    /**
    * Hash of errors (Error objects) by key
    */
    errors: {},

    /**
    * Count of errors
    */
    count: 0,

    /**
    * JQuery query for main error container 
    */
    containerQuery: "#errorBox",

    containerSuccessQuery: "#successBox",

    /**
    * Get container JQuery query
    */
    getContainerQuery: function () {
        return this.containerQuery;
    },

    /**
    * Get container JQuery query
    */
    setContainerQuery: function (query) {
        this.containerQuery = query;
    },

    getContainerSuccessQuery: function () {
        return this.containerSuccessQuery;
    },

    /**
    * Get container JQuery query
    */
    setContainerSuccessQuery: function (query) {
        this.containerSuccessQuery = query;
    },

    /**
    * Errors count
    */
    getCount: function () {
        return this.count;
    },

    /**
    * Checks that contains errors 
    */
    hasErrors: function () {
        return this.getCount() > 0;
    },

    /**
    * Add error
    */
    addError: function (error) {
        this.insure(error.getName());
        this.errors[error.getName()].push(error);
        this.count++;
    },

    /**
    * Get errors by key
    */
    getError: function (name) {
        this.insure(name);
        return this.errors[name];
    },

    /**
    * Create array
    */
    insure: function (name) {
        if (this.errors[name] == undefined)
            this.errors[name] = [];
    },

    /**
    * For each helper
    */
    forEach: function (func) {
        for (var i in this.errors) {
            var childErrors = this.errors[i];

            for (var j = 0; j < childErrors.length; j++) {
                var error = childErrors[j];
                func(error);
            }
        }
    },

    show: function (options) {

        this.clear(options);

        this.forEach(Function.reference(this, function (error) {
            if (options.errorsSummaryContainer != null) {
                $('#' + options.errorsSummaryContainer).parent().show();
                $('#' + options.errorsSummaryContainer).append('<li>' + error.getMessage() + ' </li>');
            }
            //TODO: Display error near element
            //$('#' + error.getName()).after('<div name="' + this.ERROR_NAME_ATTRIBUTE + '" class="the_error" style=color:red; padding:1px;">' + error.getMessage() + '</div>');
            
            if ($.fn.colorbox)
                $.fn.colorbox.resize();

        }));
    },

    clear: function (options) {
        if (options.errorsSummaryContainer != null) {
            $('#' + options.errorsSummaryContainer).html('');
            $('#' + options.errorsSummaryContainer).parent().hide();
        }
    }
};


/**
* Piece of html 
*/
var UpdateItem = function(query, html, updateStyle) {
    this.query = query;
    this.html = html;
    this.updateStyle = updateStyle;
};

/**
* Update styles
*/
UpdateItem.UPDATE_STYLE_REPLACE = 0;
UpdateItem.UPDATE_STYLE_APPEND = 1;
UpdateItem.UPDATE_STYLE_PREPEND = 2;
UpdateItem.UPDATE_STYLE_INSERT = 3;
UpdateItem.UPDATE_STYLE_AFTER = 4;
UpdateItem.UPDATE_STYLE_BEFORE = 5;

UpdateItem.prototype = {

    /**
    * jQuery string to find container
    */
    query: null,

    /**
    * Update style
    */
    updateStyle: 0,

    /**
    * Html
    */
    html: null,

    /**
    * jQuery query
    */
    getQuery: function() {
        return this.query;
    },

    /**
    * Html
    */
    getHtml: function() {
        return this.html;
    },

    /**
    * Update style
    */
    getUpdateStyle: function() {
        return this.updateStyle;
    }
};

/**
* Collection of UpdateItem
*/
var UpdateItems = function() {
    this.items = [];
};

UpdateItems.prototype = {
    /**
    * List of items
    */
    items: [],

    /**
    * Add item
    */
    addItem: function (item) {
        this.items.push(item);
    },

    /**
    * Checks that contains errors 
    */
    hasItems: function () {
        return this.items.length > 0;
    },

    update: function () {
        for (var i = 0; i < this.items.length; i++) {
            var item = this.items[i];
            var domElement = $(item.getQuery());
            var html = item.getHtml();

            if (item.getUpdateStyle() == UpdateItem.UPDATE_STYLE_INSERT)
                domElement.html(html);
            else if (item.getUpdateStyle() == UpdateItem.UPDATE_STYLE_REPLACE) {
                domElement.html('');
                domElement.replaceWith(html);
            }
            else if (item.getUpdateStyle() == UpdateItem.UPDATE_STYLE_APPEND)
                domElement.append(html);
            else if (item.getUpdateStyle() == UpdateItem.UPDATE_STYLE_PREPEND)
                domElement.prepend(html);
            else if (item.getUpdateStyle() == UpdateItem.UPDATE_STYLE_AFTER)
                domElement.after(html);
            else if (item.getUpdateStyle() == UpdateItem.UPDATE_STYLE_BEFORE)
                domElement.before(html);
        }
    },

    /**
    * for each helper
    */
    forEach: function (func) {
        for (var i = 0; i < this.items.length; i++) {
            var item = this.items[i];
            func(item);
        }
    }
};


/**
* Collection of UpdateItem
*/
var ResultOptions = function (data) {
    this.errorsSummaryContainer = data.Options.ErrorsSummaryContainer || "validation-summary-errors";
    this.showErrorNearElement = data.Options.ShowErrorNearElement;
    this.successSummaryContainer = data.Options.SuccessSummaryContainer || "validation-summary-success";
    this.successMessage = data.Options.SuccessMessage;
};

ResultOptions.prototype = {
    //errors summary container
    errorsSummaryContainer: null,
    
    showErrorNearElement: null,
    
    successSummaryContainer: null
};



/**
*  Result from the server
**/
function Result() {

}

/**
* Static construction from Json object
*/
Result.fromJson = function (data) {
    var result = new Result();

    //parse options
    result.options = new ResultOptions(data);
    
    // Parse errors
    var errors = new Errors();
    for (var i = 0; i < data.Errors.length; i++) {
        var error = data.Errors[i];
        errors.addError(new Error(error.Name, error.Message));
    }

    // Parse update items
    var updateItems = new UpdateItems();
    for (var i = 0; i < data.UpdateItems.length; i++) {
        var item = data.UpdateItems[i];
        updateItems.addItem(new UpdateItem(item.Query, item.Html, item.UpdateStyle));
    }

    result.setJsonItems(eval(data.Json));

    // Set errors
    result.setErrors(errors);

    // Set update items
    result.setUpdateItems(updateItems);

    // Set redirect url
    result.setRedirectUrl(data.RedirectUrl);

    // Set close popup flag
    result.setClosePopup(data.ClosePopup);

    // Set reload page flag
    result.setReloadPage(data.ReloadPage);

    return result;
};

Result.prototype = {
    /**
    * Errors object
    */
    errors: null,

    /**
    * UpdateItems object
    */
    updateItems: null,

    /**
    * Native array of associative json objects
    */
    jsonItems: null,

    /**
    * Redirect url (no redirect in case of null or empty)
    */
    redirectUrl: null,

    /**
    * Should all open popups be closed
    */
    closePopup: false,

    /**
    * Should reload page after popup closed
    */
    reloadPage: false,
    
    options: null,

    /**
    * Set redirect url
    */
    setRedirectUrl: function(url) {
        this.redirectUrl = url;
    },

    /**
    * Get redirect url    
    */
    getRedirectUrl: function() {
        return this.redirectUrl;
    },

    /**
    * Get errors
    */
    getErrors: function() {
        return this.errors;
    },

    /**
    * Check if we have errors
    */
    hasErrors: function() {
        return this.errors.hasErrors();
    },

    /**
    * Set errors object
    */
    setErrors: function(errors) {
        this.errors = errors;
    },

    /**
    * set UpdateItems object
    */
    setUpdateItems: function(items) {
        this.updateItems = items;
    },

    /**
    * get UpdateItems object
    */
    getUpdateItems: function() {
        return this.updateItems;
    },

    /**
    * Check if we have update items
    */
    hasUpdateItems: function() {
        return this.updateItems.hasItems();
    },

    /**
    * Set json items
    */
    setJsonItems: function(items) {
        for (var item in items) {
            items[item] = eval('(' + items[item] + ')');
        }
        this.jsonItems = items;
    },

    /**
    * Get json by name
    */
    getJson: function(name) {
        return this.jsonItems[name];
    },

    /**
    * Set popup flag
    */
    setClosePopup: function(flag) {
        this.closePopup = flag;
    },

    /**
    * Get popup flag
    */
    getClosePopup: function() {
        return this.closePopup;
    },

    /**
    * Set reload page flag
    */
    setReloadPage: function(flag) {
        this.reloadPage = flag;
    },

    /**
    * Get reload page flag
    */
    getReloadPage: function () {
        return this.reloadPage;
    }
}
