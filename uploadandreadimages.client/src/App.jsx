import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [imageUrl, setImageUrl] = useState("");
    const [fileName, setFileName] = useState("");
    const [fileList, setFileList] = useState([]);

    
    async function fetchImage() {
        const timestamp = new Date().getTime(); // Уникальный параметр на основе текущего времени
        const url = `https://localhost:7159/api/UploadFile/getFile/${fileName}?t=${timestamp}`;
        setImageUrl(url);
    }
    
    
    async function uploadFile(e) {
        e.preventDefault();
        const formData = new FormData(e.target);
        const response = await fetch('https://localhost:7159/api/UploadFile/uploadFile',{
            method:'POST',
            body: formData
        });
        const data = await response.json();
        const messageEl = document.querySelector(".message");
        messageEl.innerHTML = data.message
        
        console.log(data)
    }

    async function fetchAllFiles() {
        const response = await fetch('https://localhost:7159/api/UploadFile/getAllFiles');
        const files = await response.json();
    
        if (Array.isArray(files)) {
            setFileList(files.map(file => `https://localhost:7159/api/UploadFile/getFile/${file}`));
        } else {
            console.error("Error fetching files:", files.message);
        }
    }
    
    useEffect(() => {
        fetchAllFiles();
    }, []);
    return (
        <>
        <div className='file-uploader'>
            <h1>Upload files</h1>
            <p className='message'></p>

            <form action="#" method='post' onSubmit={uploadFile}>
                <label htmlFor="files">Select file(s)</label>
                <br />
                <br />
                <input type="file" name="formFiles" id="files" multiple required/>
                <br />
                <br />
                <input type="submit" value="upload" className='btn'/>
            </form>
        </div>
        <br />

        <div>  
            <label>
                Enter File Name:
                <input
                    type="text"
                    value={fileName}
                    onChange={(e) => setFileName(e.target.value)}
                    placeholder="Enter file name"
                />
            </label>
            <br />
            <button onClick={fetchImage}>Get Image</button>
            
            <br />{imageUrl && <img src={imageUrl} alt="Uploaded file" />}
        </div>

        <h2>All Images</h2>
        <div>
            {fileList.map((file, index) => (
                <img key={index} src={file} alt={`Uploaded file ${index + 1}`} style={{ width: '200px', margin: '10px' }} />
            ))}
        </div>
        </>
    ); 
}
export default App;