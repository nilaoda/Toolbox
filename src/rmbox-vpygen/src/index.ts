import * as fs from 'fs'

const helpText = `rmbox-vpygen
Usage: node rmbox-vpygen.js <input> <subtitle> <output>
`

const template = `from vapoursynth import core, YUV420P8

VIDEO_PATH = r'rmbox-input'
SUB_PATH = r'rmbox-subtitle'

clip = core.lsmas.LWLibavSource(VIDEO_PATH)
clip = core.resize.Bicubic(clip, format=YUV420P8)
clip = core.vsfm.TextSubMod(clip, SUB_PATH)
clip.set_output()
`

if (process.argv.length !== 5) {
  console.log(helpText)
  process.exit(1)
}

const inputPath = process.argv[2]
const subtitlePath = process.argv[3]
const outputPath = process.argv[4]

const result = template
  .replace('rmbox-input', inputPath)
  .replace('rmbox-subtitle', subtitlePath)

fs.writeFileSync(outputPath, result, {
  encoding: 'utf-8',
})
