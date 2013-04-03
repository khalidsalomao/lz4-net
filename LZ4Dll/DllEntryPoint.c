/*
* Entry points for the dll 
*/

#pragma once

#undef _UNICODE
#include "lz4.h"
#include "lz4hc.h"

#if defined (__cplusplus)
extern "C" {
#endif

extern __declspec(dllexport) int dll_LZ4_compress   (const char* source, char* dest, int isize)
{
	return LZ4_compress (source, dest, isize);
}

extern __declspec(dllexport) int dll_LZ4_uncompress (const char* source, char* dest, int osize)
{
	return LZ4_uncompress (source, dest, osize);
}

extern __declspec(dllexport) int dll_LZ4_compressHC (const char* source, char* dest, int isize)
{
	return LZ4_compressHC (source, dest, isize);
}

#if defined (__cplusplus)
}
#endif
