package Sers.Core.Util.Data;

import java.util.List;

public class ByteData {

//    public static int ByteDataLen(List<byte[]> byteData){
//        int len=0;
//        for ( byte[] item:byteData) {
//            len+=item.length;
//        }
//        return  len;
//    }


    public static int ByteDataLen(List<ArraySegment> byteData){
        int len=0;
        for ( ArraySegment item:byteData) {
            len+=item.count;
        }
        return  len;
    }
}
