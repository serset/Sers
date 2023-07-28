let AuthService_Config = {
    autoLogin: false,
    onLoginSuccess: (accessToken) => {
        // {access_token:'xx',expires_time:'1686857267655'}

        //set token to cookie
        let expires = new Date(parseInt(accessToken.expires_time));
        document.cookie = "Authorization=" + escape(accessToken.access_token) + ";path=/;expires=" + expires.toGMTString();
    },
    //audience: "http://localhost:4580",
    loginUrl: '/_gover_/Scripts/Vit.SSO/login.html',
    //indexUrl: '/',
    ssoBaseUrl: 'https://sso.lith.cloud:4'
};

