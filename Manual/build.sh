echo Joining all files..
utils/join.bat
echo Building PDF...
# pandoc content/*.md --toc --include-in-header header.tex -N --template eisvogel.tex -s --from markdown --highlight-style tango --variable urlcolor=cyan -o Manual.pdf
pandoc content/temp.md --toc --include-in-header header.tex -N --template eisvogel.tex -s --from markdown --highlight-style tango --variable urlcolor=cyan -o Manual.pdf
utils/cleanup.bat
echo Finished.