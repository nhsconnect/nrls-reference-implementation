/// <reference path="./node_modules/@types/node/index.d.ts" />
/**
 * To learn more about how to use Easy Webpack
 * Take a look at the README here: https://github.com/easy-webpack/core
 **/
import { generateConfig, get, stripMetadata, EasyWebpackConfig } from '@easy-webpack/core';
import * as path from 'path';

import * as envProd from '@easy-webpack/config-env-production';
import * as envDev from '@easy-webpack/config-env-development';
import * as aurelia from '@easy-webpack/config-aurelia';
import * as typescript from '@easy-webpack/config-typescript';
import * as html from '@easy-webpack/config-html';
import * as sass from '@easy-webpack/config-sass';
import * as css from '@easy-webpack/config-css';
import * as fontAndImages from '@easy-webpack/config-fonts-and-images';
import * as globalBluebird from '@easy-webpack/config-global-bluebird';
import * as globalJquery from '@easy-webpack/config-global-jquery';
import * as generateIndexHtml from '@easy-webpack/config-generate-index-html';
import * as commonChunksOptimize from '@easy-webpack/config-common-chunks-simple';
import * as copyFiles from '@easy-webpack/config-copy-files';
import * as uglify from '@easy-webpack/config-uglify';
import * as generateCoverage from '@easy-webpack/config-test-coverage-istanbul';
import * as copyWebpackPlugin from 'copy-webpack-plugin';

const ENV: string = process.env.NODE_ENV && process.env.NODE_ENV.toLowerCase() || (process.env.NODE_ENV = 'development');

/** ###################    LOOK HERE 
 * 
 *  https://github.com/aurelia/skeleton-navigation/issues/688
 */

// basic configuration:
const title = 'NRLS Demonstrator';
const baseUrl = '/';
const rootDir = path.resolve();
const srcDir = path.resolve('AppSrc');
const outDir = path.resolve('wwwroot');

const coreBundles = {
  bootstrap: [
    'aurelia-bootstrapper-webpack',
    'aurelia-polyfills',
    'aurelia-pal',
    'aurelia-pal-browser',
    'bluebird',
    'whatwg-fetch'
  ],
  // these will be included in the 'aurelia' bundle (except for the above bootstrap packages)
  aurelia: [
    'aurelia-binding',
    'aurelia-dependency-injection',
    'aurelia-event-aggregator',
    'aurelia-framework',
    'aurelia-history',
    'aurelia-history-browser',
    'aurelia-loader',
    'aurelia-loader-webpack',
    'aurelia-logging',
    'aurelia-logging-console',
    'aurelia-metadata',
    'aurelia-path',
    'aurelia-route-recognizer',
    'aurelia-router',
    'aurelia-task-queue',
    'aurelia-templating',
    'aurelia-templating-binding',
    'aurelia-templating-router',
    'aurelia-templating-resources',
    'aurelia-validation'
  ]
};

/**
 * Main Webpack Configuration
 */
let config = generateConfig(
  {
    entry: {
      'app': ['./AppSrc/main' /* this is filled by the aurelia-webpack-plugin */],
      'aurelia-bootstrap': coreBundles.bootstrap,
      'aurelia': coreBundles.aurelia.filter(pkg => coreBundles.bootstrap.indexOf(pkg) === -1)
    },
    output: {
      path: outDir,
    }
  },

  /**
   * Don't be afraid, you can put bits of standard Webpack configuration here
   * (or at the end, after the last parameter, so it won't get overwritten by the presets)
   * Because that's all easy-webpack configs are - snippets of premade, maintained configuration parts!
   * 
   * For Webpack docs, see: https://webpack.js.org/configuration/
   */

  ENV === 'test' || ENV === 'development' ? 
    envDev(ENV !== 'test' ? {} : {devtool: 'inline-source-map'}) :
    envProd({ /* devtool: '...' */ }),

  aurelia({root: rootDir, src: srcDir, title: title, baseUrl: baseUrl}),
  typescript(ENV !== 'test' ? {} : { options: { doTypeCheck: false, sourceMap: false, inlineSourceMap: true, inlineSources: true } }),
  html(),
  copyFiles({
      patterns: [
          { from: 'webappconfig' + (ENV === 'development' ? '' : '.' + ENV) + '.json', to: 'webappconfig.json' },
          { from: 'Resources/*.docx', to: './' },
          { from: 'images', to: 'images' }
      ]
  }),
  sass({ filename: 'app.[contenthash].css', allChunks: true, sourceMap: false}),
  css({ filename: 'lib.[contenthash].css', allChunks: true, sourceMap: false }),
  fontAndImages(),
  globalBluebird(),
  globalJquery(),
  generateIndexHtml({minify: ENV === 'production'}),
  
  ...(ENV === 'production' || ENV === 'development' ? [
      commonChunksOptimize({appChunkName: 'app', firstChunk: 'aurelia-bootstrap'})/*,
      copyFiles({patterns: [{ from: 'favicon.ico', to: 'favicon.ico' }]})*/
    ] : [
    /* ENV === 'test' 
    generateCoverage({ options: { esModules: true } })*/
  ]),

  ENV === 'production' ?
    uglify({debug: false, mangle: { except: ['cb', '__webpack_require__'] }}) : {}
);

module.exports = stripMetadata(config);
