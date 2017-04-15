[CmdletBinding()]
param (
    [Parameter(Mandatory=$True,Position=1)][string]$source,
    [Parameter(Mandatory=$True)][string]$destination
 )

function CopyFiles([string]$source, [string]$destination) {

	Copy-Item $source $destination
}

CopyFiles $source $destination