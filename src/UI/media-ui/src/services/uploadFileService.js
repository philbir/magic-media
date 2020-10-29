import http from "../http-client";

export const uploadFile = (file, onUploadProgress) => {
    let formData = new FormData();
    formData.append("file", file);

    return http.post("/media/upload", formData, {
      headers: {
        "Content-Type": "multipart/form-data"
      },
      onUploadProgress
    });
  }

