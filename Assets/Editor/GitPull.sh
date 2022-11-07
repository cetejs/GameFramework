#!/bin/sh

pull_branch="#pull_branch#"

git stash
git pull origin $pull_branch
git stash pop