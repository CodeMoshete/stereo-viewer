const chalk = require('chalk');
const debug = require('debug')('stereo-pipeline');
const path = require('path');// Load configuration
const fs = require('fs');

global.appRoot = path.resolve(__dirname);

function GenerateManifestEntry(baseFileName, extension) {
  const manifestEntry =
  {
    Source: 0,
    LeftImagePath: `${baseFileName}_L.${extension}`,
    RightImagePath: `${baseFileName}_R.${extension}`,
    Distance: 0,
    Divergence: 0
  };
  return manifestEntry;
}

debug(`Stereo Viewer manifest generator tool!\nArgs:\n${JSON.stringify(process.argv, null, 2)}`);
const folderPath = process.argv[2];
const manifestPath = `${folderPath}/manifest.json`;
const manifestData = JSON.parse(fs.readFileSync(manifestPath, 'utf-8'));
debug(`Manifest Data: ${JSON.stringify(manifestData, null, 2)}`);
const imageList = manifestData.Images;
const existingManifestFiles = [];
for (let i = 0, count = imageList.length; i < count; i += 1) {
  const fileName = imageList[i].LeftImagePath;
  const baseFileName = fileName.split('.')[0].split('_')[0];
  if (!existingManifestFiles.includes(baseFileName)) {
    existingManifestFiles.push(baseFileName);
  }
}

const fileNames = fs.readdirSync(folderPath);
debug(`Files in target folder: ${JSON.stringify(fileNames, null, 2)}`);

for (let i = 0, count = fileNames.length; i < count; i += 1) {
  const fileName = fileNames[i];
  debug(`Scanning file: ${fileName}`);
  const isCorrectExtension = fileName.endsWith('.jpg') || fileName.endsWith('.png');
  const isCorrectNamingConvention = fileName.includes('_L.') || fileName.includes('_R.');
  if (isCorrectExtension && isCorrectNamingConvention) {
    const fileParts = fileName.split('.');
    const extension = fileParts[1];
    const baseFileName = fileParts[0].split('_')[0];
    if (!existingManifestFiles.includes(baseFileName)) {
      debug(chalk.cyan(`New image found: ${baseFileName}`));
      const manifestEntry = GenerateManifestEntry(baseFileName, extension);
      imageList.push(manifestEntry);
      existingManifestFiles.push(baseFileName);
    }
  }
}

const manifestNewPath = `${folderPath}/manifest_new.json`;
fs.writeFileSync(manifestNewPath, JSON.stringify(manifestData, null, 2));
debug(chalk.green('Done!'));
