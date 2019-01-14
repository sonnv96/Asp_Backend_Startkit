$(function () {
    $('#input_apiKey').attr("placeholder", "token");
    $('#input_apiKey').off();
    $('#input_apiKey').change(function () {
        var token = this.value;
        if (token && token.trim() !== '') {
            //token = 'Bearer ' + token;
            var apiKeyAuth = new window.SwaggerClient.ApiKeyAuthorization("Authorization", token, "header");
            window.swaggerUi.api.clientAuthorizations.add("token", apiKeyAuth);
        }
    });
})();