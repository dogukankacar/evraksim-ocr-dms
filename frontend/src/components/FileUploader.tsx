"use client";

import { useRef, useState } from "react";

interface FileUploaderProps {
  onFileSelect: (file: File) => void;
  accept?: string;
}

export default function FileUploader({
  onFileSelect,
  accept = ".pdf,.docx,.doc,.jpg,.jpeg,.png",
}: FileUploaderProps) {
  const [fileName, setFileName] = useState<string>("");
  const [dragActive, setDragActive] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];
      setFileName(file.name);
      onFileSelect(file);
    }
  };

  const handleDrag = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === "dragenter" || e.type === "dragover") {
      setDragActive(true);
    } else if (e.type === "dragleave") {
      setDragActive(false);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);
    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      const file = e.dataTransfer.files[0];
      setFileName(file.name);
      onFileSelect(file);
    }
  };

  return (
    <div
      onDragEnter={handleDrag}
      onDragLeave={handleDrag}
      onDragOver={handleDrag}
      onDrop={handleDrop}
      onClick={() => inputRef.current?.click()}
      className={`border-2 border-dashed rounded-lg p-8 text-center cursor-pointer transition ${
        dragActive
          ? "border-blue-500 bg-blue-50"
          : "border-gray-300 hover:border-blue-400"
      }`}
    >
      <input
        ref={inputRef}
        type="file"
        accept={accept}
        onChange={handleChange}
        className="hidden"
      />
      {fileName ? (
        <p className="text-gray-700">{fileName}</p>
      ) : (
        <div>
          <p className="text-gray-500 mb-1">
            Dosyayı sürükleyin veya tıklayarak seçin
          </p>
          <p className="text-xs text-gray-400">
            PDF, DOCX, DOC, JPG, JPEG, PNG (maks. 10MB)
          </p>
        </div>
      )}
    </div>
  );
}
