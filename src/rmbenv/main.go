package main

import (
    "fmt"
    "log"
    "os"
    "os/exec"
    "path"
    "path/filepath"
    "runtime"
)

func main() {
    var suffix string
    if runtime.GOOS == "windows" {
        suffix = ".exe"
    } else {
        suffix = ""
    }
    filename := "rmbox/rmbox" + suffix

    basePath, err := filepath.Abs(filepath.Dir(os.Args[0]))

    if err != nil {
        log.Printf("解析进程路径 %v 时出现错误。", filename)
        fmt.Println(err)
        b := make([]byte, 1)
        _, _ = os.Stdin.Read(b)
        os.Exit(1)
    }

    filename = path.Join(basePath, filename)

    _, err = os.Stat(filename)

    if err != nil {
        log.Printf("未寻找到 %v 文件。", filename)
        fmt.Println(err)
        b := make([]byte, 1)
        _, _ = os.Stdin.Read(b)
        os.Exit(1)
    }

    cmd := exec.Command(filename, os.Args[1:]...)
    cmd.Stderr = os.Stderr
    cmd.Stdout = os.Stdout

    _ = cmd.Run()
}
