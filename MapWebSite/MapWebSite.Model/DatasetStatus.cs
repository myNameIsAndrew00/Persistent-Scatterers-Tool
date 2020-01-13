using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.Model
{
    public enum DatasetStatus
    {

        [EnumString("None")]
        //Represents dataset status not set
        None,

        [EnumString("Uploaded")]
        //This status means that dataset was uploaded but not processed by the service
        Uploaded = 1,

        [EnumString("Generated")]
        //This status means that the dataset was uploaded and generated in database
        Generated = 2,

        [EnumString("Pending")]
        //This status means that the dataset was only proposed to be uploaded 
        Pending = 3,

        //This status means that the dataset has an error during uploading

        [EnumString("Upload failure")]
        UploadFail = 4
    }
}
