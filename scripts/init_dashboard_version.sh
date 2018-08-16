#!/bin/sh
#
# prints the dashboard version data as a comma-separated list of values, including a header
#

FILENAME="dashboard_version.csv"

[[ -n $DEBUG ]] && set -x

set -eu -o pipefail

DEST=$1

if [[ ! -d $DEST ]]; then
    echo "$DEST must be a directory"
    exit 1
fi

git clone https://github.com/dfds/ded-grafana-dashboards.git $DEST

cd $DEST

cat > $FILENAME << EOF
entry;hash;author_name;author_email;author_date;committer_name;committer_email;committer_date;message
EOF

for f in *.json; do
    git log -1 --pretty=format:"$f;%H;%an;%aE;%ai;%cn;%cE;%ci;%s%n" -- $f >> $FILENAME
done
