export function setCookie(name, value, maxAge, domain, path) {
    var cookie = name + '=' + value + '; max-age=' + maxAge + '; path=' + path;
    if (domain != '') {
        cookie += '; domain=' + domain;
    }

    document.cookie = cookie;
}

export function setSessionCookie(name, value, domain, path) {
    var cookie = name + '=' + value + '; path=' + path;
    if (domain != '') {
        cookie += '; domain=' + domain;
    }

    document.cookie = cookie;
}

export function getCookie(name) {
    let matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}

export function removeCookie(name, domain = '/', path = '/') {
    var cookie = name + '=; max-age:-1';
    cookie += '; domain=' + domain + '; path=' + path;

    document.cookie = cookie;
}