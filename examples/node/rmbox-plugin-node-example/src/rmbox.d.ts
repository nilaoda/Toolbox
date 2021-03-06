export interface Meta {
  id: string
  class_name: string
  name: string
  description: string
  config_sections: string[]
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  [K: string]: any
}

export interface Command {
  target: string
  args: string
}
