/**
* Single error message 
**/
var AttachedControls = function(element) {
    this.controls = {};
};

/**
* Attach id
*/
AttachedControls.ATTACHES_ID = "ATTACHES_ID";

/**
* Get Attaches object from element or elementId
*/
AttachedControls.from = function(obj) {
    // if string don't starts from # - add this symbol
    if (Object.isString(obj)) {
        if (obj.indexOf('#') != 0) {
            obj = '#' + obj;
        }
    }

    var element = Object.isString(obj) ? $(obj) : obj;

    if (!$(element).data(AttachedControls.ATTACHES_ID)) {
        $(element).data(AttachedControls.ATTACHES_ID, new AttachedControls());
    }

    return $(element).data(AttachedControls.ATTACHES_ID);
}

AttachedControls.prototype = {

    controls: {},

    attach: function(id, object) {
        this.controls[id] = object;
    },

    get: function(id) {
        return this.controls[id];
    },

    /**
    * for each helper
    */
    forEach: function(func) {
        for (i in this.controls) {
            var control = this.controls[i];
            func(control);
        }
    },

    refresh: function() {
        this.forEach(function(control) {
            control.refresh();
        });
    }
};