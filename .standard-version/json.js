module.exports.readVersion = function (contents) {
    return JSON.parse(contents)["resource"];
}

module.exports.writeVersion = function (contents, version) {
    let r = JSON.parse(contents);
    r.resource = version;

    return JSON.stringify(r, null, 4);
}