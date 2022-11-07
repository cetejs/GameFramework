#!/bin/sh

clear_branch="#clear_branch#"

git checkout --orphan temp
git add -a
git commit -am "#cmt_message#"
git branch -d $clear_branch
git branch -m $clear_branch
git push -f origin $clear_branch