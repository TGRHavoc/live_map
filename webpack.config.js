const webpack = require("webpack");
const path = require("path");
const nodeExternals = require("webpack-node-externals");

const config = {
    entry: "./src/server.js",
    devtool: false,
    mode: "production",
    output: {
        path: path.resolve(__dirname, "dist"),
        filename: "livemap.js"
    },
    resolve: {
        alias: {
            LivemapSocketController: path.resolve(__dirname, "src", "sockets.js"),
            LivemapBlipController: path.resolve(__dirname, "src", "blips.js"),
            LivemapEventsWrapper: path.resolve(__dirname, "src", "wrapper.js")
        }
    },
    externals: [nodeExternals()],
    plugins: [
        new webpack.DefinePlugin({ "global.GENTLY": false })
    ],
    optimization: {
        minimize: false
    },
    target: "node"
};

module.exports = config;