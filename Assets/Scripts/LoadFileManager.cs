using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading;

public class LoadFileManager : MonoBehaviour 
{

    //we use some integer to get error codes from the lzma library (look at lzma.cs for the meaning of these error codes)
    private int zres = 0;

    private string myFile;
    private WWW www;

    private string log;

    private string ppath;

    private bool compressionStarted, pass;
    private bool downloadDone;

    //reusable buffers
    private byte[] reusableBuffer, reusableBuffer2, reusableBuffer3;

    //fixed size buffers, that don't get resized, to perform compression/decompression of buffers in them and avoid memory allocations.
    private byte[] fixedInBuffer = new byte[1024 * 256];
    private byte[] fixedOutBuffer = new byte[1024 * 768];
    private byte[] fixedBuffer = new byte[1024 * 1024];

    //A single item integer array that changes to the current number of file that get uncompressed of a zip archive.
    //When running the decompress_File function, compare this int to the total number of files returned by the getTotalFiles function
    //to get the progress of the extraction if the zip contains multiple files.
    //If you use multiple threads, remember to use other progress integers for the other threads, otherwise there will be a sharing violation.
    //
    private int[] progress = new int[1];

    //individual file progress (in bytes)
    private int[] progress2 = new int[1];



	void Start () 
    {
        ppath = Application.temporaryCachePath;
        Debug.Log(ppath);

        //various byte buffers for testing
        reusableBuffer = new byte[4096];
        reusableBuffer2 = new byte[0];
        reusableBuffer3 = new byte[0];

        myFile = "challenge-bundle.zip";


        PrintTemporaryCachePath();

	}
	
	void Update () 
    {
		
	}


    public void onFileLoadButtonClicked()
    {
        
        Debug.Log("onFileLoadButtonClicked");

        StartCoroutine(DownloadFile());
    }

    public void onExpandFileButtonClicked()
    {

        Debug.Log("onExpandFileButtonClicked");


        DoDecompression();

    }


    private void PrintPersistentDataPath()
    {
        Debug.Log("persistentDataPath" + Application.persistentDataPath);
    }

    private void PrintTemporaryCachePath()
    {
        Debug.Log("temporaryCachePath" + Application.temporaryCachePath);
    }



    IEnumerator DownloadFile()
    {
        var uwr = new UnityWebRequest("https://s3.amazonaws.com/master-chef-debug/asset-bundle-zips/challenge-bundle.zip", UnityWebRequest.kHttpVerbGET);
        //string path = Path.Combine(Application.persistentDataPath, "challenge-bundle.zip");
        string path = Path.Combine(Application.temporaryCachePath, "challenge-bundle.zip");
        uwr.downloadHandler = new DownloadHandlerFile(path);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError)
            Debug.LogError(uwr.error);
        else
            Debug.Log("File successfully downloaded and saved to " + path);
    }




    void DoDecompression()
    {



        //validate sanity of a zip archive
        Debug.Log("Validate: " + lzip.validateFile(ppath + "/challenge-bundle.zip").ToString());


        //decompress the downloaded file
        zres = lzip.decompress_File(ppath + "/challenge-bundle.zip", ppath + "/", progress, null, progress2);
        Debug.Log("decompress: " + zres.ToString());
        Debug.Log("");

        //get the true total files of the zip
        Debug.Log("true total files: " + lzip.getTotalFiles(ppath + "/challenge-bundle.zip"));

    }






}
