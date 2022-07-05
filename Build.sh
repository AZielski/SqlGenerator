#!/bin/bash

#Variables
EalierDir="$(ls | grep build_win64)"
EalierBuildDate="$(echo $EalierDir | cut -d'-' -f2)"
Start=$(date +%s.%N)

echo "Starting build of application 😊"

if dotnet build . --sc -v m;
then
	#Variables
	ToPath="./build_win64-$(date +%Y%m%d%H%M%S)"
	SummaryFile="$ToPath/summary.txt"
	
	#Move build to ./ and change its name
	rm -rf "./$EalierDir"
	echo
	echo -e "\033[33;32m---------------------------------------"
	echo "Moving build to $ToPath"
	mv "./App/bin/Debug/net6.0/win-x64/" "$ToPath"
	echo "Directory moved to $ToPath"
	echo "---------------------------------------"
	echo
	#Remove last_succesfull if exists
	echo "---------------------------------------"
	echo "Removing all last_succesfull builds"
	rm -rf ./last_succesfull-*
	echo "Removed all last_succesfull builds"
	echo "---------------------------------------"
	
	#Create summary file
	echo
	echo "---------------------------------------"
	echo "Creating summary.txt inside build"
	touch $SummaryFile
	echo "UUID: $(uuidgen)" >> $SummaryFile
	echo "Date and time: $(date)" >> $SummaryFile
	echo "Hash SHA512: $(echo "$(date +%Y%m%d%H%M%S)$(neofetch)$(ls -la $ToPath)" | openssl sha512 | cut -d' ' -f2) " >> $SummaryFile
	echo "Created summary.txt file"
	echo "---------------------------------------"
else
	echo -e "\033[33;31m---------------------------------------"
	
	if [ -z "$EalierDir" ];
	then
		#Print that error occured and ealier success build does not exists
		echo "There are still errors and there is no build to back up"
	else
		#Backs up ealier successfull build.
		LastSuccesfull="./last_succesfull-$EalierBuildDate"
		echo "Build did not succesed. 🙁"
		mv "$EalierDir" "$LastSuccesfull"
		echo "Last build available in $LastSuccesfull"
	fi
	
	echo "---------------------------------------"
fi

duration=$(echo "$(date +%s.%N) - $Start" | bc)
echo
echo "Full duration time was: $duration"
