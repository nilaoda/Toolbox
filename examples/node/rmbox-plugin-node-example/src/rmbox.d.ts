/* eslint-disable @typescript-eslint/no-explicit-any */

export interface Meta {
  id: string
  class_name: string
  name: string
  type: string
  description: string
  config_sections: { [K: string]: any }
  [K: string]: any
}

export interface Command {
  target: string
  args: string
}
