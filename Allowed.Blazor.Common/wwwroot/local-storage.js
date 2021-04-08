export function getLocal(name) {
    var result = localStorage.getItem(name);
    if (result)
        return result;
    return "";
}

export function setLocal(name, value) {
    localStorage.setItem(name, value);
}