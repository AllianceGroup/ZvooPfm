'use strict';

angular.module('app.affiliateAdmin').service('messagesService', ['http', function(http){

    this.getMessages = function(){
        var url = "/Messages";
        return http.get(url);
    };

    this.editMessage = function(id){
        var url = "/Messages/edit" + http.buildQueryString({id : id});
        return http.get(url);
    };

    this.updateMessage = function(message){
        var url = "/Messages/update";
        return http.post(url, message);
    };

    this.deleteMessage = function(id){
        var url = "/Messages/delete/" + id;
        return http.delete(url);
    };

    this.previewMessage = function(message){
        var url = '/Messages/previewMessage';
        return http.post(url, message);
    };

    this.getTemplates = function(){
        var url = "/Templates";
        return http.get(url);
    };

    this.updateTemplate = function(template){
        var url = "/Templates/update";
        return http.post(url, template);
    };

    this.editTemplate = function(id){
        var url = '/Templates/edit' + http.buildQueryString({id: id});
        return http.get(url);
    };

    this.deleteTemplate = function(id){
        var url = '/Templates/delete/' + id;
        return http.delete(url);
    };

    this.getFaq = function () {
        var url = "/Faq";
        return http.get(url);
    };

    this.updateFaq = function (template) {
        var url = "/Faq/update";
        return http.post(url, template);
    };

    this.editFaq = function (id) {
        var url = '/Faq/edit' + http.buildQueryString({ id: id });
        return http.get(url);
    };

    this.deleteFaq = function (id) {
        var url = '/Faq/delete/' + id;
        return http.delete(url);
    };

    this.getSegments = function(){
        var url = "/CampaignBuilder/getSegments";
        return http.get(url);
    };

    this.getSegment = function(){
        var url = "/CampaignBuilder/createSegment";
        return http.get(url);
    };

    this.createSegment = function(segment){
        var url = "/CampaignBuilder/createSegment";
        return http.post(url, segment);
    };

    this.editSegment = function(id){
        var url = "/CampaignBuilder/editSegment/" + id;
        return http.get(url);
    };

    this.clearFilters = function(id){
        var url = "/CampaignBuilder/clearFilters" + http.buildQueryString({segmentId: id});
        return http.get(url);
    };

    this.updateSegment = function(segment){
        var url = "/CampaignBuilder/editSegment";
        return http.post(url, segment);
    };

    this.estimateSegment = function(segment){
        var url = "/CampaignBuilder/estimateSegment";
        return http.post(url, segment);
    };

    this.deleteSegment = function(id){
        var url = "/CampaignBuilder/deleteSegment/" + id;
        return http.delete(url);
    };

    this.getMail = function(){
        var url = '/Mail';
        return http.get(url);
    };

    this.sendMail = function(mail){
        var url = '/Mail';
        return http.post(url, mail);
    };

    this.getTriggers = function(){
        var url = '/Triggers';
        return http.get(url);
    };

    this.editTrigger = function(id){
        var url = '/Triggers/editTrigger/' + id;
        return http.get(url);
    };

    this.updateTrigger = function(trigger){
        var url = '/Triggers/editTrigger';
        return http.post(url, trigger);
    };
}]);