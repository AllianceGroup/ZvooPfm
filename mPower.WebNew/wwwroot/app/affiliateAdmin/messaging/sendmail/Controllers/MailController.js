"use strict";

angular.module('app.affiliateAdmin').controller('MailController', function SendMailController(model, messagesService, $stateParams, authService){
    var ctrl = this;
    ctrl.model = model;
    ctrl.errors = [];
    ctrl.model.Ids = "";
    ctrl.ids = [];
    ctrl.roles = authService.getRoles();

    if($stateParams.Id)
        ctrl.ids[ctrl.ids.length] = $stateParams.Id;

    ctrl.send = function(){
        for(var i = 0; i < ctrl.ids.length; i++){
            ctrl.model.Ids += ctrl.ids[i] + ',';
        }

        messagesService.sendMail(ctrl.model).then(function(){
            ctrl.errors = [];
            $.smallBox({
                title: "Success",
                content: "Your send request was successfully registered and related email will be send as soon as possible.",
                color: "#739e73",
                timeout: 10000
            });
        }, function(errors){
            ctrl.errors = _.reduce(errors, function(result, arr) {
                return result.concat(arr);
            }, []);
        });
    }
});