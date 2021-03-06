import * as fs from 'fs'
import { Command, Meta } from './rmbox'

const meta: Meta = {
  id: 'Ruminoid.Toolbox.Plugins.Examples.RmboxPluginNodeExample',
  class_name: 'RmboxPluginNodeExample',
  name: 'Node 插件示例',
  description: 'Node 编写的 Ruminoid Toolbox 插件示例。',
  config_sections: [
    'Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection',
  ],
}

if (process.argv.length == 1) {
  console.log(JSON.stringify(meta))
  process.exit(0)
}

const configSectionData: string = fs.readFileSync(process.argv[1], {
  encoding: 'utf-8',
})
const configSections = JSON.parse(configSectionData)
const ioSection =
  configSections[
    'Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection'
  ]

const commands: Command[] = [
  {
    target: 'ffmpeg',
    args: `-i ${ioSection['video']} ${ioSection['output']}`,
  },
]

console.log(JSON.stringify(commands))
