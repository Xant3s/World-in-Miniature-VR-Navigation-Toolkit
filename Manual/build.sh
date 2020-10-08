echo Building PDF...
# pandoc content/*.md --toc --include-in-header header.tex -N --template eisvogel.tex -s --from markdown --highlight-style tango --variable urlcolor=cyan -o Manual.pdf
# pandoc content/temp.md --toc --include-in-header header.tex -N --template eisvogel.tex -s --from markdown --highlight-style tango --variable urlcolor=cyan -o Manual.pdf
pandoc content/*.md --toc --include-in-header header.tex -N --template eisvogel.tex -s --from markdown --highlight-style tango --variable urlcolor=cyan -o Manual.pdf && cp Manual.pdf ../Assets/WIMVR/Manual.pdf
# rm ../Assets/WIMVR/Manual.pdf
# rm ../Assets/WIMVR/Manual.pdf.meta
cp Manual.pdf ../Assets/WIMVR/Manual.pdf
echo Finished.