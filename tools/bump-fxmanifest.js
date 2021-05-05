//version "2.3.2"

const versionRegex = new RegExp("^version \"(.+)\"", "gm");

module.exports.readVersion = function (contents) {
    let matches = contents.match(versionRegex);
    let realversion = matches[0].match("\"(.+)\"")[1];
    return realversion;
};

module.exports.writeVersion = function (contents, version) {
    let t = contents;
    t = t.replace(versionRegex, `version "${version}"`);
    return t;
};