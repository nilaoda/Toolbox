json = require 'json'

local meta = {
    id = 'Ruminoid.Toolbox.Plugins.Examples.RmboxPluginLuaExample',
    class_name = 'RmboxPluginLuaExample',
    name = 'Lua 插件示例',
    type = 'operation',
    description = 'Lua 编写的 Ruminoid Toolbox 插件示例。',
    author = 'Il Harper',
    config_sections = {
        ["Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"] = {}
    }
}

if (type(arg[1]) == "nil") then
    print(json.encode(meta))
    os.exit(0)
end

io.input(arg[1])
local config_section_data = io.read("a")

local config_sections = json.decode(config_section_data)
local io_section =
    config_sections["Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"]

local commands = {
    target = "ffmpeg",
    args = "-i " + io_section["input"] + " " + io_section["output"],
    formatter = "ffmpeg"
}

print(json.encode(commands))
