const regex = new RegExp("<Version>(.+)</Version>");
const fileReg = new RegExp("<FileVersion>.+</FileVersion>");

module.exports.readVersion = function(contents) {
    return regex.exec(contents)[1];
}

module.exports.writeVersion = function(contents, version){
    let replaced = contents.replace(regex, `<Version>${version}</Version>`);
    replaced = replaced.replace(fileReg, `<FileVersion>${version}</FileVersion>`);

    return replaced;
}
