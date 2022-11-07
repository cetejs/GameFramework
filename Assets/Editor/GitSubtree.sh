#!/bin/sh

split_path="#split_path#"
split_branch="#split_branch#"

git subtree split --prefix=$split_path --branch $split_branch
git push origin $split_branch