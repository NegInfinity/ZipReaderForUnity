using System.Collections.Generic;
using System.IO;

namespace Zip{
	enum Signatures{
		LocalFile = 0x04034b50,
		CentralDirectoryFile = 0x02014b50,
		DataDescriptor = 0x08074b50,
		EndOfCentralDirectory = 0x06054b50
	}
	
	enum CompressionMethod{
		None = 0x0,
		Deflate = 0x8
	}

	static partial class ReaderExtensions{
		static public string readFixedString(this BinaryReader r, int stringSize){
			var bytes = r.ReadBytes(stringSize);
			var res = System.Text.Encoding.UTF8.GetString(bytes);
			return res;
		}
	}
	
	public struct LocalHeader{	
		public uint signature;
		public ushort minVersion;
		public ushort bitFlag;
		public ushort compressionMethod;
		public ushort lastModificationTime;
		public ushort lastModificationDate;
		public uint crc32;
		public uint compressedSize;
		public uint uncompressedSize;
		public ushort filenameLength;
		public ushort extraFieldLength;
		public string filename;
		public byte[] extraField;
				
		public bool hasValidSiganture(){
			return signature == (uint)Signatures.LocalFile;
		}
		
		public static int getFixedPartSize(){
			return 30;
		}
		
		public int getVariablePartSize(){
			return filenameLength + extraFieldLength;
		}
		
		public void loadFixedPart(BinaryReader r){
			signature = r.ReadUInt32();
			minVersion = r.ReadUInt16();
			bitFlag = r.ReadUInt16();
			compressionMethod = r.ReadUInt16();
			lastModificationTime = r.ReadUInt16();
			lastModificationDate = r.ReadUInt16();
			crc32 = r.ReadUInt32();
			compressedSize = r.ReadUInt32();
			uncompressedSize = r.ReadUInt32();
			filenameLength = r.ReadUInt16();
			extraFieldLength = r.ReadUInt16();
		}
		
		public void loadVariablePart(BinaryReader r){
			filename = r.readFixedString(filenameLength);
			extraField = r.ReadBytes(extraFieldLength);
		}
		
		public void load(BinaryReader r){
			loadFixedPart(r);
			loadVariablePart(r);
		}
	}	
	
	public struct DataDescriptor{
		public uint crc32;
		public uint compressedSize;
		public uint uncompressedSize;
		
		public void load(BinaryReader r){
			crc32 = r.ReadUInt32();
			if (crc32 == (uint)Signatures.DataDescriptor)
				crc32 = r.ReadUInt32();
			compressedSize = r.ReadUInt32();
			uncompressedSize = r.ReadUInt32();
		}
	};
	
	static partial class ReaderExtensions{
		public static DataDescriptor readDataDescriptor(this BinaryReader r){
			var res = new DataDescriptor();
			res.load(r);
			return res;
		}
		
		public static LocalHeader readLocalZipHeader(this BinaryReader r){
			var res = new LocalHeader();
			res.load(r);
			return res;
		}
	}
	
	public struct CentralDirectoryFileHeader{
		public uint signature;
		public ushort versionMadeBy;
		public ushort versionToExtract;
		public ushort bitFlag;
		public ushort compressionMethod;
		public ushort lastModificationTime;
		public ushort lastModificationDate;
		public uint crc32;
		public uint compressedSize;
		public uint uncompressedSize;
		public ushort filenameLength;
		public ushort extraFieldLength;
		public ushort fileCommentLength;
		public ushort fileDiskNumber;
		public ushort fileInternalAttributes;
		public uint externalFileAttributes;
		public uint fileHeaderOffset;
		public string filename;
		public byte[] extraField;
		public string comment;
		
		public override string ToString(){
			var sb = new System.Text.StringBuilder();
			sb.AppendFormat("[CentralDirectoryFileHeader]");
			sb.AppendFormat("signature: {0}; ", signature);
			sb.AppendFormat("versionMadeBy: {0}; ", versionMadeBy);
			sb.AppendFormat("versionToExtract: {0}; ", versionToExtract);
			sb.AppendFormat("bitFlag: {0}; ", bitFlag);
			sb.AppendFormat("compressionMethod: {0}; ", compressionMethod);
			sb.AppendFormat("lastModificationTime: {0}; ", lastModificationTime);
			sb.AppendFormat("lastModificationDate: {0}; ", lastModificationDate);
			sb.AppendFormat("crc32: {0}; ", crc32);
			sb.AppendFormat("compressedSize: {0}; ", compressedSize);
			sb.AppendFormat("uncompressedSize: {0}; ", uncompressedSize);
			sb.AppendFormat("filenameLength: {0}; ", filenameLength);
			sb.AppendFormat("extraFieldLength: {0}; ", extraFieldLength);
			sb.AppendFormat("fileCommentLength: {0}; ", fileCommentLength);
			
			sb.AppendFormat("fileDiskNumber: {0}; ", fileDiskNumber);
			sb.AppendFormat("fileInternalAttributes: {0}; ", fileInternalAttributes);
			
			sb.AppendFormat("externalFileAttributes: {0}; ", externalFileAttributes);
			sb.AppendFormat("fileHeaderOffset: {0}; ", fileHeaderOffset);
			
			sb.AppendFormat("filename: {0}; ", filename);
			sb.AppendFormat("extraField: {0}; ", extraField);
			sb.AppendFormat("comment: {0}; ", comment);
			
			//return string.Format("[CentralDirectoryFileHeader]");
			return sb.ToString();
		}
		
		public bool hasValidSignature(){
			return signature == (uint)Signatures.CentralDirectoryFile;
		}
		
		public static int getFixedPartSize(){
			return 46;
		}
		
		public int getVariablePartSize(){
			return filenameLength + extraFieldLength + fileCommentLength;
		}
		
		public void loadFixedPart(BinaryReader r){
			signature = r.ReadUInt32();
			versionMadeBy = r.ReadUInt16();
			versionToExtract = r.ReadUInt16();
			bitFlag = r.ReadUInt16();
			compressionMethod = r.ReadUInt16();
			lastModificationTime = r.ReadUInt16();
			lastModificationDate = r.ReadUInt16();
			crc32 = r.ReadUInt32();
			compressedSize = r.ReadUInt32();
			uncompressedSize = r.ReadUInt32();
			filenameLength = r.ReadUInt16();
			extraFieldLength = r.ReadUInt16();
			fileCommentLength = r.ReadUInt16();
			fileDiskNumber = r.ReadUInt16();
			fileInternalAttributes = r.ReadUInt16();
			externalFileAttributes = r.ReadUInt32();
			fileHeaderOffset = r.ReadUInt32();
		}
		
		public void loadVariablePart(BinaryReader r){
			filename = r.readFixedString(filenameLength);
			extraField = r.ReadBytes(extraFieldLength);
			comment = r.readFixedString(fileCommentLength);			
		}
		
		public void load(BinaryReader r){
			loadFixedPart(r);
			loadVariablePart(r);
		}
	}

	public struct EndOfCentralDirectoryRecord{
		public uint signature;
		public ushort diskNumber;
		public ushort directoryStartDiskNumber;
		public ushort diskNumberOfCentralDirectories;
		public ushort totalNumberOfDirectories;
		public uint directorySize;
		public uint directoryOffset;
		public ushort commentLength;
		public string comment;
		
		public override string ToString(){
			return string.Format("[EndOfCentralDirectoryRecord]: signature: {0}; diskNumber: {1}; startDisk: {2}; diskNumDirectories: {3}; " +
				"totalDirectories: {4}; " + 
				"directorySize: {5}; directoryOffset: {6}; commentLength: {7}; comment: \"{8}\"",
				signature, diskNumber, directoryStartDiskNumber, diskNumberOfCentralDirectories, totalNumberOfDirectories, 
				directorySize, directoryOffset, commentLength, comment);
		}
		
		public bool hasValidSignature(){
			return signature == (uint)Signatures.EndOfCentralDirectory;
		}
		
		public int getVariablePartSize(){
			return commentLength;
		}
		
		public static int getFixedPartSize(){
			return 22;
		}
		
		public void loadFixedPart(BinaryReader r){
			signature = r.ReadUInt32();
			diskNumber = r.ReadUInt16();
			directoryStartDiskNumber = r.ReadUInt16();
			diskNumberOfCentralDirectories = r.ReadUInt16();
			totalNumberOfDirectories = r.ReadUInt16();
			directorySize = r.ReadUInt32();
			directoryOffset = r.ReadUInt32();
			commentLength = r.ReadUInt16();
		}
		
		public void loadVariablePart(BinaryReader r){
			comment = r.readFixedString(commentLength);
		}
		
		public void load(BinaryReader r){
			loadFixedPart(r);
			loadVariablePart(r);
		}
	}
	
	static partial class ReaderExtensions{
		public static CentralDirectoryFileHeader readCentralDirectoryFileHeader(this BinaryReader r){
			var res = new CentralDirectoryFileHeader();
			res.load(r);
			return res;
		}
		
		public static EndOfCentralDirectoryRecord readEndOfCentralDirectoryRecord(this BinaryReader r){
			var res = new EndOfCentralDirectoryRecord();
			res.load(r);
			return res;
		}
	}
	
	public class ZipReader: System.IDisposable{
		string archiveFileName;
		EndOfCentralDirectoryRecord eocd;
		BinaryReader reader = null;
		long archiveSize = 0;
		//long eocdOffset = 0;	
		long directoryOffset = 0;
		long directorySize = 0;
		
		List<CentralDirectoryFileHeader> fileHeaders = new List<CentralDirectoryFileHeader>();
		Dictionary<string, int> fileDict = new Dictionary<string, int>();
		
		public CentralDirectoryFileHeader getFileRecord(int fileIndex){
			return fileHeaders[fileIndex];
		}
		
		public List<string> getFiles(){
			return new List<string>(fileDict.Keys);
		}
		
		string convertToDictFileName(string filename){
			return filename.ToLower();
		}
		
		public bool hasFile(string filename){
			var dictFilename = convertToDictFileName(filename);
			return fileDict.ContainsKey(dictFilename);
		}
		
		public int numFiles{
			get{
				return fileHeaders.Count;
			}
		}
		
		int getFileIndex(string filename, bool throwIfNotFound = false){
			var normFilename = convertToDictFileName(filename);
			int result = -1;
			if (!fileDict.TryGetValue(normFilename, out result))
				return -1;
			if (throwIfNotFound){
				if ((result < 0) || (result >= fileHeaders.Count)){
					throw new System.ArgumentException(
						string.Format("Filename \"{0}\" not found in archive \"{1}\"", filename, archiveFileName)
					);
				}
			}
			return result;
		}
		
		public byte[] decompressFile(string filename){
			int fileIndex = getFileIndex(filename, true);
			return decompressFile(fileHeaders[fileIndex]);
		}
		
		public byte[] decompressFile(CentralDirectoryFileHeader inHeader){			
			streamPosition = inHeader.fileHeaderOffset;
			var localHeader = new LocalHeader();
			localHeader.load(reader);
			
			var compressedBytes = reader.ReadBytes((int)inHeader.compressedSize);
			if (inHeader.compressionMethod == 0)
				return compressedBytes;
				
			if (inHeader.compressionMethod != 8){//deflate
				throw new System.ArgumentException(
					string.Format("Unsupported compression method {0} in file \"{1}\" of archive \"{2}\"",
						inHeader.filename, inHeader.compressionMethod, archiveFileName));
			}
			
			var tmp = new byte[65536];
			using(var memStream = new System.IO.MemoryStream(compressedBytes))
			using(var deflateStream = new System.IO.Compression.DeflateStream(memStream, System.IO.Compression.CompressionMode.Decompress))
			using(var outStream = new System.IO.MemoryStream()){
				int bytesRead;
				while((bytesRead = deflateStream.Read(tmp, 0, tmp.Length)) > 0){
					outStream.Write(tmp, 0, bytesRead);
				}
				return outStream.ToArray();
			}
		}
		
		public byte[] loadFile(string filename){
			var fileIndex = getFileIndex(filename, true);
			return decompressFile(fileHeaders[fileIndex]);
		}
		
		public void load(string filename){
			reader = new BinaryReader(File.Open(filename, FileMode.Open));
			archiveFileName = filename;
			archiveSize = getStreamSize();
			if (!seekEocd()){
				throw new System.ArgumentException(string.Format("Could not locate eocd in \"{0}\"", archiveFileName));
			}
			
			//Debug.LogFormat("Archive Eocd found: {0}", eocd);
			
			directoryOffset = eocd.directoryOffset;
			directorySize = eocd.directorySize;
			if ((directoryOffset + directorySize) > archiveSize){
				throw new System.ArgumentException(string.Format("Invalid directory size in  \"{0}\"", archiveFileName));
			}
			
			loadDirectory();
		}
		
		public void Dispose(){
			if (reader != null){
				((System.IDisposable)reader).Dispose();
			}
		}
		
		long streamPosition{
			get{
				return reader.BaseStream.Position;
			}
			set{
				reader.BaseStream.Position = value;
			}
		}
		
		void loadDirectory(){
			var maxOffset = directoryOffset + directorySize;
			streamPosition = directoryOffset;
			int recordIndex = 0;
			fileHeaders.Clear();
			fileDict.Clear();
			while(streamPosition < maxOffset){
				var curRec = new CentralDirectoryFileHeader();
				curRec.loadFixedPart(reader);
				if (!curRec.hasValidSignature())
					throw new System.ArgumentException(
						string.Format("Invalid signature in record {0}, file \"{1}\"", recordIndex, archiveFileName));
				if ((curRec.getVariablePartSize() + streamPosition) > maxOffset)
					throw new System.ArgumentException(
						string.Format("Record {0} too big in file \"{1}\"", recordIndex, archiveFileName)
					);
				curRec.loadVariablePart(reader);
				
				//Debug.LogFormat("Record {0}: {1}", recordIndex, curRec);
				
				fileHeaders.Add(curRec);
				fileDict.Add(convertToDictFileName(curRec.filename), recordIndex);				
				recordIndex++;					
			}
		}
		
		long getStreamSize(){
			var oldPos = reader.BaseStream.Position;
			var endPos = reader.BaseStream.Seek(0, SeekOrigin.End);
			reader.BaseStream.Position = oldPos;
			return endPos;
		}
		
		public bool seekEocd(){
			var baseRecordSize = EndOfCentralDirectoryRecord.getFixedPartSize();
			var maxOffset = archiveSize - baseRecordSize;
			
			for(var baseOffset = maxOffset; baseOffset > 0; baseOffset--){
				reader.BaseStream.Position = baseOffset;
				eocd.loadFixedPart(reader);
				if (!eocd.hasValidSignature())
					continue;
				if ((eocd.getVariablePartSize() + baseRecordSize) > archiveSize)
					continue;
				eocd.loadVariablePart(reader);
				//eocdOffset = baseOffset;
				return true;
			}
					
			return false;
		}
	}
}
