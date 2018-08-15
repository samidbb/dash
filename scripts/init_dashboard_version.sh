#!/bin/bash
#
# prints the dashboard version data as a comma-separated list of values, including a header
#

[[ -n $DEBUG ]] && set -x

set -eu -o pipefail

cat << EOF
entry;hash;author_name;author_email;author_date;committer_name;committer_email;committer_date;message
EOF

for f in *.json; do
    git log -1 --pretty=format:"$f;%H;%an;%aE;%ai;%cn;%cE;%ci;%s%n" -- $f
done
