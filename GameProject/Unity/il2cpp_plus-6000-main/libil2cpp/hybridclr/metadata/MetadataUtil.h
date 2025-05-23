#pragma once
#include <tuple>

#include "vm/GlobalMetadata.h"
#include "vm/Exception.h"
#include "utils/HashUtils.h"
#include "metadata/Il2CppTypeHash.h"
#include "metadata/Il2CppTypeCompare.h"

#include "../CommonDef.h"
#include "MetadataDef.h"

namespace hybridclr
{
namespace metadata
{
    class Image;

#pragma region byteorder

    template<int N>
    inline void* GetAlignBorder(const void* pointer)
    {
        uint64_t p = (uint64_t)pointer;
        if (p % N == 0)
        {
            return (void*)pointer;
        }
        else
        {
            return (void*)((p + N - 1) / N * N);
        }
    }

    inline int32_t GetI1(const byte* data)
    {
        return *(int8_t*)data;
    }

    inline int16_t GetI2LittleEndian(const byte* data)
    {
#if SUPPORT_MEMORY_NOT_ALIGMENT_ACCESS
        uint16_t value = *(uint16_t*)data;
#else
        uint16_t value = (uint16_t)data[0] | ((uint16_t)data[1] << 8);
#endif
        return (int16_t)value;
    }

    inline uint16_t GetU2LittleEndian(const byte* data)
    {
        return (uint16_t)GetI2LittleEndian(data);
    }

    inline int32_t GetI4LittleEndian(const byte* data)
    {
#if SUPPORT_MEMORY_NOT_ALIGMENT_ACCESS
        uint32_t value = *(uint32_t*)data;
#else
        uint32_t value = (uint32_t)data[0]
            | ((uint32_t)data[1] << 8)
            | ((uint32_t)data[2] << 16)
            | ((uint32_t)data[3] << 24);
#endif
        return (int32_t)value;
    }

    inline int64_t GetI8LittleEndian(const byte* data)
    {
#if SUPPORT_MEMORY_NOT_ALIGMENT_ACCESS
        uint64_t value = *(uint64_t*)data;
#else
        uint64_t value = (uint64_t)data[0]
            + ((uint64_t)data[1] << 8)
            + ((uint64_t)data[2] << 16)
            + ((uint64_t)data[3] << 24)
            + ((uint64_t)data[4] << 32)
            + ((uint64_t)data[5] << 40)
            + ((uint64_t)data[6] << 48)
            + ((uint64_t)data[7] << 56);
#endif
        return value;
    }

    uint32_t GetNotZeroBitCount(uint64_t x);

#pragma endregion


#pragma region interpreter metadtata index

    const uint32_t kMetadataIndexBits = 22;

    const uint32_t kMetadataKindBits = 2;

    const uint32_t kMetadataKindShiftBits = 32 - kMetadataKindBits;

    const uint32_t kMetadataImageIndexShiftBits = kMetadataIndexBits;

    const uint32_t kMetadataImageIndexExtraShiftBitsA = 6;
    const uint32_t kMetadataImageIndexExtraShiftBitsB = 4;
    const uint32_t kMetadataImageIndexExtraShiftBitsC = 2;
    const uint32_t kMetadataImageIndexExtraShiftBitsD = 0;
    extern const uint32_t kMetadataImageIndexExtraShiftBitsArr[4];

    const uint32_t kMetadataIndexMaskA = (1 << (kMetadataIndexBits + kMetadataImageIndexExtraShiftBitsA)) - 1;
    const uint32_t kMetadataIndexMaskB = (1 << (kMetadataIndexBits + kMetadataImageIndexExtraShiftBitsB)) - 1;
    const uint32_t kMetadataIndexMaskC = (1 << (kMetadataIndexBits + kMetadataImageIndexExtraShiftBitsC)) - 1;
    const uint32_t kMetadataIndexMaskD = (1 << (kMetadataIndexBits + kMetadataImageIndexExtraShiftBitsD)) - 1;
    extern const uint32_t kMetadataIndexMaskArr[4];

    const uint32_t kMetadataImageIndexBits = 32 - kMetadataIndexBits;

    const uint32_t kMaxMetadataImageCount = (1 << kMetadataImageIndexBits);

    const uint32_t kMaxMetadataImageIndexWithoutKind = 1u << (kMetadataImageIndexBits - kMetadataKindBits);

    const uint32_t kInvalidImageIndex = 0;

    const int32_t kInvalidIndex = -1;

    inline int32_t DecodeMetadataKind(uint32_t index)
    {
		return index >> kMetadataKindShiftBits;
	}

    inline uint32_t DecodeImageIndex(int32_t index)
    {
        if (index == kInvalidIndex)
        {
			return 0;
		}
        uint32_t uindex = (uint32_t)index;
        uint32_t kind = uindex >> kMetadataKindShiftBits;
        return (uindex & ~kMetadataIndexMaskArr[kind]) >> kMetadataImageIndexShiftBits;
    }

    inline uint32_t DecodeMetadataIndex(int32_t index)
    {
        if (index == kInvalidIndex)
        {
            return kInvalidIndex;
        }
        uint32_t uindex = (uint32_t)index;
        uint32_t kind = uindex >> kMetadataKindShiftBits;
        return uindex & kMetadataIndexMaskArr[kind];
    }

    inline int32_t EncodeImageAndMetadataIndex(uint32_t imageIndex, int32_t rawIndex)
    {
        if (rawIndex == kInvalidIndex)
        {
			return kInvalidIndex;
		}
        IL2CPP_ASSERT(((imageIndex << kMetadataImageIndexShiftBits) & (uint32_t)rawIndex) == 0);
        return (imageIndex << kMetadataIndexBits) | (uint32_t)rawIndex;
    }

    inline bool IsInterpreterIndex(int32_t index)
    {
        //return DecodeImageIndex(index) != 0;
        return index != kInvalidIndex && ((uint32_t)index & ~kMetadataIndexMaskA) != 0;
    }

    inline bool IsInterpreterType(const Il2CppTypeDefinition* typeDefinition)
    {
        return IsInterpreterIndex(typeDefinition->byvalTypeIndex);
    }

    inline bool IsInterpreterType(const Il2CppClass* klass)
    {
        return IsInterpreterIndex(klass->image->token) && klass->rank == 0;
    }

    inline bool IsInterpreterImage(const Il2CppImage* image)
    {
        return IsInterpreterIndex(image->token);
    }

    inline bool IsPrologHasThis(uint32_t flags)
    {
        return flags & 0x20;
    }

    inline bool IsPrologExplicitThis(uint32_t flags)
    {
        return flags & 0x40;
    }

#pragma endregion


#pragma region method and klass

    inline bool IsInstanceField(const Il2CppType* type)
    {
        return (type->attrs & FIELD_ATTRIBUTE_STATIC) == 0;
    }

    inline bool IsInterpreterMethod(const MethodInfo* method)
    {
        return IsInterpreterType(method->klass);
    }

    inline bool IsInterpreterMethod(const Il2CppMethodDefinition* method)
    {
        return IsInterpreterIndex(method->declaringType);
    }

    inline bool IsInterpreterImplement(const MethodInfo* method)
    {
        return method->isInterpterImpl;
    }

    inline bool IsInstanceMethod(const MethodInfo* method)
    {
        return !(method->flags & METHOD_ATTRIBUTE_STATIC);
    }

    inline bool IsInstanceMethod(const Il2CppMethodDefinition* method)
    {
        return !(method->flags & METHOD_ATTRIBUTE_STATIC);
    }

    inline bool IsStaticMethod(const MethodInfo* method)
    {
        return (method->flags & METHOD_ATTRIBUTE_STATIC);
    }

    inline bool IsPrivateMethod(uint32_t flags)
    {
        return (flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PRIVATE;
    }

    inline bool IsPublicMethod(uint32_t flags)
    {
        return (flags & METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == METHOD_ATTRIBUTE_PUBLIC;
    }

    inline bool IsGenericIns(const Il2CppType* type)
    {
        return type->type == IL2CPP_TYPE_GENERICINST;
    }

    inline bool IsVirtualMethod(uint32_t flags)
    {
        return flags & METHOD_ATTRIBUTE_VIRTUAL;
    }

    inline bool IsAbstractMethod(uint32_t flags)
    {
        return flags & METHOD_ATTRIBUTE_ABSTRACT;
    }

    inline bool IsNewSlot(uint32_t flags)
    {
        return flags & METHOD_ATTRIBUTE_NEW_SLOT;
    }

    inline bool IsSealed(uint32_t flags)
    {
        return flags & METHOD_ATTRIBUTE_FINAL;
    }

    inline bool IsInterface(uint32_t flags)
    {
        return flags & TYPE_ATTRIBUTE_INTERFACE;
    }

	inline bool IsPInvokeMethod(uint32_t flags)
	{
		return flags & METHOD_ATTRIBUTE_PINVOKE_IMPL;
	}

#define IMPLMAP_FLAG_NOT_MANGLE 0x1

#define IMPLMAP_FLAG_CHARSET_MASK 0x6
#define IMPLMAP_FLAG_CHARSET_NOT_SPECIFIED 0x0
#define IMPLMAP_FLAG_CHARSET_ANSI 0x2
#define IMPLMAP_FLAG_CHARSET_UNICODE 0x4
#define IMPLMAP_FLAG_CHARSET_AUTO 0x6

#define IMPLMAP_FLAG_SUPPORTS_LAST_ERROR 0x40

#define IMPLMAP_FLAG_CALLCONV_MASK 0x700
#define IMPLMAP_FLAG_CALLCONV_PLATFORMAPI 0x100
#define IMPLMAP_FLAG_CALLCONV_CDECL 0x200
#define IMPLMAP_FLAG_CALLCONV_STDCALL 0x300
#define IMPLMAP_FLAG_CALLCONV_THISCALL 0x400
#define IMPLMAP_FLAG_CALLCONV_FASTCALL 0x500


	inline bool IsDllImportNoMangle(uint32_t mappingFlags)
	{
		return mappingFlags & IMPLMAP_FLAG_NOT_MANGLE;
	}

    inline Il2CppCharSet GetDllImportCharSet(uint32_t mappingFlags)
    {
        uint32_t charSet = mappingFlags & IMPLMAP_FLAG_CHARSET_MASK;
        switch (charSet)
        {
		case IMPLMAP_FLAG_CHARSET_NOT_SPECIFIED:
			return Il2CppCharSet::CHARSET_NOT_SPECIFIED;
		case IMPLMAP_FLAG_CHARSET_ANSI:
			return Il2CppCharSet::CHARSET_ANSI;
		case IMPLMAP_FLAG_CHARSET_UNICODE:
			return Il2CppCharSet::CHARSET_UNICODE;
        default:
            IL2CPP_ASSERT(false);
			return Il2CppCharSet::CHARSET_NOT_SPECIFIED;
        }
    }

    inline Il2CppCallConvention GetDllImportCallConvention(uint32_t mappingFlags)
    {
		uint32_t callConv = mappingFlags & IMPLMAP_FLAG_CALLCONV_MASK;
        switch (callConv)
        {
        case IMPLMAP_FLAG_CALLCONV_PLATFORMAPI: return Il2CppCallConvention::IL2CPP_CALL_DEFAULT;
		case IMPLMAP_FLAG_CALLCONV_CDECL: return Il2CppCallConvention::IL2CPP_CALL_C;
		case IMPLMAP_FLAG_CALLCONV_STDCALL: return Il2CppCallConvention::IL2CPP_CALL_STDCALL;
		case IMPLMAP_FLAG_CALLCONV_THISCALL: return Il2CppCallConvention::IL2CPP_CALL_THISCALL;
		case IMPLMAP_FLAG_CALLCONV_FASTCALL: return Il2CppCallConvention::IL2CPP_CALL_FASTCALL;
		default:
			IL2CPP_ASSERT(false);
			return Il2CppCallConvention::IL2CPP_CALL_DEFAULT;
        }
    }

    bool IsValueType(const Il2CppType* type);

    inline bool IsValueType(const Il2CppTypeDefinition* typeDef)
    {
        return typeDef->bitfield & (1 << (il2cpp::vm::kBitIsValueType - 1));
    }

    inline bool IsEnumType(const Il2CppTypeDefinition* typeDef)
    {
        return (typeDef->bitfield >> (il2cpp::vm::kBitIsEnum - 1)) & 0x1;
    }

    inline const Il2CppTypeDefinition* GetUnderlyingTypeDefinition(const Il2CppType* type)
    {
        if (IsGenericIns(type))
        {
            return (Il2CppTypeDefinition*)type->data.generic_class->type->data.typeHandle;
        }
        else
        {
            return (Il2CppTypeDefinition*)type->data.typeHandle;
        }
    }

    const Il2CppType* GetIl2CppTypeFromTypeDefinition(const Il2CppTypeDefinition* typeDef);

    inline uint32_t GetActualArgumentNum(const MethodInfo* method)
    {
        return (uint32_t)method->parameters_count + (!(method->flags & METHOD_ATTRIBUTE_STATIC));
    }

    inline bool IsReturnVoidMethod(const MethodInfo* method)
    {
        return method->return_type->type == IL2CPP_TYPE_VOID;
    }

    inline bool IsVoidType(const Il2CppType* type)
    {
        return type->type == IL2CPP_TYPE_VOID;
    }

    inline const MethodInfo* GetUnderlyingMethodInfo(const MethodInfo* method)
    {
        return !method->genericMethod || method->is_generic ? method : method->genericMethod->methodDefinition;
    }

    inline bool IsChildTypeOfMulticastDelegate(const Il2CppClass* klass)
    {
        return klass->parent == il2cpp_defaults.multicastdelegate_class;
    }

    inline int32_t GetActualParamCount(const MethodInfo* methodInfo)
    {
        return IsInstanceMethod(methodInfo) ? (methodInfo->parameters_count + 1) : methodInfo->parameters_count;
    }

    inline int32_t GetFieldOffset(const FieldInfo* fieldInfo)
    {
        Il2CppClass* klass = fieldInfo->parent;
        return IS_CLASS_VALUE_TYPE(klass) ? (fieldInfo->offset - sizeof(Il2CppObject)) : fieldInfo->offset;
    }

    inline int32_t GetThreadStaticFieldOffset(const FieldInfo* fieldInfo)
    {
        return il2cpp::vm::MetadataCache::GetThreadLocalStaticOffsetForField(const_cast<FieldInfo*>(fieldInfo));
    }

    const Il2CppType* TryInflateIfNeed(const Il2CppType* selfType, const Il2CppGenericContext* genericContext, bool inflateMethodVars);
    const Il2CppType* TryInflateIfNeed(const Il2CppType* containerType, const Il2CppType* selfType);

    bool IsTypeSameByTypeIndex(TypeIndex t1, TypeIndex t2);

    bool IsTypeEqual(const Il2CppType* t1, const Il2CppType* t2);

    bool IsTypeGenericCompatible(const Il2CppType* t1, const Il2CppType* t2);

    bool IsOverrideMethod(const Il2CppType* type1, const Il2CppMethodDefinition* method1, const Il2CppType* type2, const Il2CppMethodDefinition* method2);
    bool IsOverrideMethodIgnoreName(const Il2CppType* type1, const Il2CppMethodDefinition* methodDef1, const Il2CppType* type2, const Il2CppMethodDefinition* methodDef2);

    const Il2CppMethodDefinition* ResolveMethodDefinition(const Il2CppType* type, const char* resolveMethodName, const MethodRefSig& resolveSig);

    const MethodInfo* GetMethodInfoFromMethodDef(const Il2CppType* type, const Il2CppMethodDefinition* methodDef);

    bool ResolveField(const Il2CppType* type, const char* resolveFieldName, const Il2CppType* resolveFieldType, const Il2CppFieldDefinition*& retFieldDef);

    inline void ResolveFieldThrow(const Il2CppType* type, const char* resolveFieldName, const Il2CppType* resolveFieldType, const Il2CppFieldDefinition*& retFieldDef)
    {
        if (!ResolveField(type, resolveFieldName, resolveFieldType, retFieldDef))
        {
            RaiseMissingFieldException(type, resolveFieldName);
        }
    }

    const Il2CppGenericContainer* GetGenericContainerFromIl2CppType(const Il2CppType* type);

    inline const Il2CppGenericContainer* GetGenericContainer(const MethodInfo* methodDef)
    {
        return methodDef->is_inflated ?
            (const Il2CppGenericContainer*)methodDef->genericMethod->methodDefinition->genericContainerHandle :
            (const Il2CppGenericContainer*)methodDef->genericContainerHandle;
    }

    bool IsMatchSigType(const Il2CppType* dstType, const Il2CppType* sigType, const Il2CppGenericContainer* klassGenericContainer, const Il2CppGenericContainer* methodGenericContainer);

    bool IsMatchMethodSig(const Il2CppMethodDefinition* methodDef, const MethodRefSig& resolveSig, const Il2CppGenericContainer* klassGenericContainer);
    bool IsMatchMethodSig(const MethodInfo* methodDef, const MethodRefSig& resolveSig, const Il2CppGenericContainer* klassGenericContainer);
    bool IsMatchMethodSig(const MethodInfo* methodDef, const MethodRefSig& resolveSig, const Il2CppType** klassInstArgv, const Il2CppType** methodInstArgv);

    const Il2CppGenericInst* TryInflateGenericInst(const Il2CppGenericInst* inst, const Il2CppGenericContext* genericContext);

	bool HasNotInstantiatedGenericType(const Il2CppType* type);
    bool HasNotInstantiatedGenericType(const Il2CppGenericInst* inst);

#pragma endregion


#pragma region misc

    int32_t GetTypeValueSize(const Il2CppType* type);

    inline int32_t GetTypeValueSize(const Il2CppClass* klass)
    {
        if (IS_CLASS_VALUE_TYPE(klass))
        {
            return il2cpp::vm::Class::GetValueSize((Il2CppClass*)klass, nullptr);
        }
        else
        {
            return sizeof(Il2CppObject*);
        }
    }

    inline int32_t GetStackSizeByByteSize(int32_t size)
    {
        return (size + 7) / 8;
    }

    inline int32_t GetTypeValueStackObjectCount(const Il2CppType* type)
    {
        return (GetTypeValueSize(type) + 7) / 8;
    }

    inline void RaiseBadImageException(const char* msg = nullptr)
    {
        il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetBadImageFormatException(msg));
    }
#pragma endregion

    class Il2CppTypeHashShallow
    {
    public:
        size_t operator()(const Il2CppType* t1) const
        {
            size_t h = (size_t)t1->data.dummy;
            h = il2cpp::utils::HashUtils::Combine(h, t1->attrs);
            h = il2cpp::utils::HashUtils::Combine(h, (size_t)t1->type);
            h = il2cpp::utils::HashUtils::Combine(h, t1->byref);
            h = il2cpp::utils::HashUtils::Combine(h, t1->pinned);
#if HYBRIDCLR_UNITY_2021_OR_NEW
            h = il2cpp::utils::HashUtils::Combine(h, t1->valuetype);
#endif
            return h;
        }
    };

    class Il2CppTypeEqualityComparerShallow
    {
    public:
        bool operator()(const Il2CppType* t1, const Il2CppType* t2) const
        {
            return (t1->data.dummy == t2->data.dummy)
                && t1->type == t2->type
                && t1->attrs == t2->attrs
                && t1->byref == t2->byref
                && t1->pinned == t2->pinned
#if HYBRIDCLR_UNITY_2021_OR_NEW
                && t1->valuetype == t2->valuetype
#endif
                ;
        }
    };

}
}