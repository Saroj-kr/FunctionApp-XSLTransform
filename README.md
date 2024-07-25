# Function App for XSLT Transformation

This repo contains Blob-Triggered Function App code for XSLT Transformation.
There are function apps for both In-Process and Isolated Models.
You can use either Isolated or In-Process variant of C# code in Azure.

## Inputs
The FunctionApp needs following inputs from App Settions:
```json
{
    "xsltransformcontainer": "xsltransform",    // Blob storage container used for all operations
    "mapName": "map.xslt",                      // XSLT file stored in container root folder
    "destinationFolder": "destination",         // Directory for storing XSLT output
    "sourceFolder": "source"                    // Directory under watch for new input files 
}
```

## How does it work
- The FunctionApp monitors `xsltransformcontainer/sourceFolder` for any new XML file.
- Whenever a new file is detected, the FunctionApp is triggered which reads the file stream,
- It also reads the XSLT file with name `mapName` from `xsltransformcontainer` container,
- Then it performs the XSLT transformation and places the output file in `xsltransformcontainer/destinationFolder` with name `<SourceFileName>-output.xml`.

## Authorization
- FunctionApp currently uses Azure Storage connection string for read/write operation in Blob Storage. 

