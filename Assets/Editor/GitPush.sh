#!/bin/sh

dir_path="#dir_path#"
push_branch="#push_branch#"

git add $dir_path
git commit -m "#cmt_message#"
git push origin $push_branch