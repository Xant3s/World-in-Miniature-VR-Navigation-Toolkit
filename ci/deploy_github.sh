#!/usr/bin/env bash

OIFS="$IFS"
IFS=$'\n'

echo "Deploying to Github..."
REPO="WIMVR-Deployment"

# Delete all files that no longer exist
cd ..
cd ${REPO}
if ! test -f .gitattributes; then
    rm .gitattributes
fi

if ! test -f .gitignore; then
    rm .gitignore
fi

for f in $(find ./ProjectSettings -type f); do
    if ! test -f "../$f"; then
        rm "$f"
    fi
done

for f in $(find ./Packages -type f); do
    if ! test -f "../$f"; then
        rm "$f"
    fi
done

for f in $(find ./Assets/WIMVR -type f); do
    if ! test -f "../$f"; then
        rm "$f"
    fi
done


# Copy all new files
cd ..
if ! test -f ${REPO}/.gitattributes; then
    cp .gitattributes ${REPO}/.gitattributes
fi

if ! test -f ${REPO}/.gitignore; then
    cp .gitignore ${REPO}/.gitignore
fi
for f in $(find ProjectSettings -type f); do
    if ! test -f "${REPO}/$f"; then
        mkdir -p "$(dirname "${REPO}/$f")"
        cp "$f" "${REPO}/$f"
    fi
done

for f in $(find Packages -type f); do
    if ! test -f "${REPO}/$f"; then
        mkdir -p "$(dirname "${REPO}/$f")"
        cp "$f" "${REPO}/$f"
    fi
done

for f in $(find Assets/WIMVR -type f); do
    if ! test -f "${REPO}/$f"; then
        mkdir -p "$(dirname "${REPO}/$f")"
        cp "$f" "${REPO}/$f"
    fi
done


# Copy all modifies file
cmp --silent .gitattributes ${REPO}/.gitattributes || cp .gitattributes ${REPO}/.gitattributes
cmp --silent .gitattributes ${REPO}/.gitignore || cp .gitattributes ${REPO}/.gitignore

for f in $(find ./ProjectSettings -type f); do
    mkdir -p "$(dirname "${REPO}/$f")"
    cmp --silent "$f" "${REPO}/$f" || cp "$f" "${REPO}/$f"
done

for f in $(find ./Packages -type f); do
    mkdir -p "$(dirname "${REPO}/$f")"
    cmp --silent "$f" "${REPO}/$f" || cp "$f" "${REPO}/$f"
done

for f in $(find ./Assets/WIMVR -type f); do
    mkdir -p "$(dirname "${REPO}/$f")"
    cmp --silent "$f" "${REPO}/$f" || cp "$f" "${REPO}/$f"
done