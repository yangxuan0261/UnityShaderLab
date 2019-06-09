#!/usr/bin/env python
# -*- coding: utf-8 -*-

import os
import sys
import platform

# import git

SelfPath = sys.path[0]
PCName = platform.node()


def gitUpdate(path):
    print("--- 当前更新路径：%s" % path)
    os.system("git pull --progress -v origin master:master")
    os.system("git add -A")
    os.system("git commit -am 'Robot-%s'" % (PCName))
    os.system("git push origin master:master")


def doneTips():
    tips = """ 
---      ┏┛ ┻━━━━━┛ ┻┓          
---      ┃　　　　　　┃         
---      ┃　　　━　　 ┃          
---      ┃  ┳┛　  ┗┳  ┃         
---      ┃　　　　　　┃          
---      ┃　　　┻　　 ┃        
---      ┃　　　　　　┃       
---      ┗━┓　　　┏━━━┛         
---        ┃　　　┃   
---        ┃　　　┃   提交完毕！       
---        ┃　　　┗━━━━━━━━━┓       
---        ┃　　　　　　　  ┣┓        
---        ┃　　　　       ┏┛         
---        ┗━┓ ┓ ┏━━━┳ ┓ ┏━┛        
---          ┃ ┫ ┫   ┃ ┫ ┫          
---          ┗━┻━┛   ┗━┻━┛          
"""
    print(tips)
    pass


def doneTips2():
    print("------------ 提交完毕 ------------")


def main():
    print("------ 提交者：%s" % PCName)
    print("------ 提交路径：%s" % SelfPath)
    print("")

    os.chdir(SelfPath)
    gitUpdate(SelfPath)
    print("")

    # doneTips()
    doneTips2()
    pass


if __name__ == "__main__":
    main()
    # os.system("ls")
    # os.system("pause")
    sys.exit(0)
