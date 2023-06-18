/* ========================================================================
 * Author  : Lith
 * Version : 1.3
 * Date    : 2023-06-18
 * Email   : serset@yeah.net
 * ======================================================================== */

/* AuthService.js

    <script src="./AuthService.js" makeSureLogin="true"></script>

    <script type="text/javascript" >
       let token = authService.accessToken.access_token;

       authService.logoff();
       authService.makeSureLogin();
    </script>
 */

; ((window) => {

    window.authStore = new AuthStore();
    window.authService = new AuthService();


    function AuthStore() {

        // getQueryString('name')    getQueryString("aaa.html?a=1&amp;b=2",'name')
        this.getQueryString = function (key, src) {
            if (!src) src = location.search;
            var v = (src.match(new RegExp("(?:\\?|&)" + key + "=(.*?)(?=&|$)")) || ['', null])[1];
            return v && decodeURIComponent(v);
        };

        this.clearToken = function () {
            localStorage.removeItem('jwt_token');
            localStorage.removeItem('jwt_token_expires_time');
        };

        this.cacheToken = function (accessToken) {
            localStorage.setItem('jwt_token', accessToken.access_token);
            let expires_time = parseInt(accessToken.expires_time);
            if (!expires_time && accessToken.expires_in) {
                let expires_in = accessToken.expires_in - 10;
                expires_time = accessToken.expires_time = new Date().getTime() + expires_in * 1000;
            }
            if (!expires_time || expires_time <= new Date().getTime()) {
                return null;
            }
            localStorage.setItem('jwt_token_expires_time', expires_time);

            return {
                access_token: localStorage.getItem('jwt_token'),
                expires_time: localStorage.getItem('jwt_token_expires_time')
            };
        };

        //{access_token:'xx',expires_time:'1686857267655'}
        this.getToken = function () {
            var expires_time = parseInt(localStorage.getItem('jwt_token_expires_time'));
            if (expires_time) {
                if (expires_time <= new Date().getTime()) {
                    localStorage.removeItem('jwt_token');
                    localStorage.removeItem('jwt_token_expires_time');
                }
                else {
                    return {
                        access_token: localStorage.getItem('jwt_token'),
                        expires_time: localStorage.getItem('jwt_token_expires_time')
                    };
                }
            }
            return null;
        };
    }



    function AuthService() {
        this.audience;
        this.loginUrl = '/login.html';
        this.indexUrl = '/';
        this.ssoBaseUrl = 'https://sso.lith.cloud';
        this.onLoginSuccess = null;

        this.accessToken = null;
        let self = this;
        function jumpToSsoLogin() {
            //'https://sso.vit.com.cn/connect/authorize?client_id=Vit.SSO.Example&redirect_uri=http://localhost:5000&response_type=token&scope=openid profile email phone&state=123&nonce=456&audience=common';
            let url = self.ssoBaseUrl + '/login.html?redirect_uri=' + encodeURIComponent(location.href);
            if (self.audience) url += '&audience=' + encodeURIComponent(self.audience);
            window.location.href = url;
        }


        function jumpToPrevUrl() {
            let redirect_uri = localStorage.getItem('jwt_redirect_uri');
            if (redirect_uri) {
                localStorage.removeItem('jwt_redirect_uri');
                window.location.href = redirect_uri;
            } else {
                window.location.href = self.indexUrl;
            }
        };


        //  authClient.login();
        this.login = function () {

            let access_token = authStore.getQueryString('access_token');
            if (access_token) {
                var expires_time = authStore.getQueryString('expires_time');
                this.accessToken = authStore.cacheToken({ access_token, expires_time });
                if (this.accessToken) {
                    try {
                        if (this.onLoginSuccess) this.onLoginSuccess(this.accessToken);
                    } catch (e) {
                        console.log(e);
                    }
                    jumpToPrevUrl();
                    return this.accessToken;
                }
            }
            this.accessToken = authStore.getToken();
            if (this.accessToken) {
                jumpToPrevUrl();
                return this.accessToken;
            }
            jumpToSsoLogin();
            return false;
        };

        this.logoff = function (redirect_uri) {
            this.accessToken = null;
            authStore.clearToken();
            if (!redirect_uri) redirect_uri = new URL(this.indexUrl, location.origin).toString();
            window.location.href = self.ssoBaseUrl + '/logoff.html?redirect_uri=' + encodeURIComponent(redirect_uri);
        };




        //  authService.makeSureLogin();
        this.makeSureLogin = function () {
            this.accessToken = authStore.getToken();
            if (this.accessToken) {
                return this.accessToken;
            }
            localStorage.setItem('jwt_redirect_uri', location.href);
            window.location.href = this.loginUrl;
            return false;
        };

    }

    try {
        let config = (typeof (AuthService_Config) == 'object') ? AuthService_Config : {};

        if (config.loginUrl) authService.loginUrl = config.loginUrl;
        if (config.indexUrl) authService.indexUrl = config.indexUrl;
        if (config.onLoginSuccess) authService.onLoginSuccess = config.onLoginSuccess;
        if (config.ssoBaseUrl) authService.ssoBaseUrl = config.ssoBaseUrl;
        if (config.audience) authService.audience = config.audience;

        if ('true' == document.currentScript.getAttribute('makeSureLogin')) {
            if (config.autoLogin !== false)
                authService.makeSureLogin();
        } else if ('true' == document.currentScript.getAttribute('login')) {
            authService.login();
        }
    } catch (e) {
        console.log(e);
    }

})(window);