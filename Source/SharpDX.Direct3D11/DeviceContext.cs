﻿// Copyright (c) 2010-2011 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Runtime.InteropServices;

namespace SharpDX.Direct3D11
{
    public partial class DeviceContext
    {
        /// <summary>
        ///   Constructs a new deferred context <see cref = "T:SharpDX.Direct3D11.DeviceContext" />.
        /// </summary>
        /// <param name = "device">The device with which to associate the state object.</param>
        /// <returns>The newly created object.</returns>
        public DeviceContext(Device device)
            : base(IntPtr.Zero)
        {
            DeviceContext temp;
            device.CreateDeferredContext(0, out temp);
            NativePointer = temp.NativePointer;
        }

        /// <summary>
        ///   Create a command list and record graphics commands into it.
        /// </summary>
        /// <param name = "restoreState">A flag indicating whether the immediate context state is saved (prior) and restored (after) the execution of a command list.</param>
        /// <returns>The created command list containing the queued rendering commands.</returns>
        public CommandList FinishCommandList(bool restoreState)
        {
            CommandList temp;
            FinishCommandListInternal(restoreState, out temp);
            return temp;
        }

        /// <summary>
        ///   Gets data from the GPU asynchronously.
        /// </summary>
        /// <param name = "data">The asynchronous data provider.</param>
        /// <returns>The data retrieved from the GPU.</returns>
        public DataStream GetData(Asynchronous data)
        {
            return GetData(data, AsynchronousFlags.None);
        }

        /// <summary>
        ///   Gets data from the GPU asynchronously.
        /// </summary>
        /// <param name = "data">The asynchronous data provider.</param>
        /// <returns>The data retrieved from the GPU.</returns>
        public T GetData<T>(Asynchronous data) where T : struct
        {
            return GetData<T>(data, AsynchronousFlags.None);
        }

        /// <summary>
        ///   Gets data from the GPU asynchronously.
        /// </summary>
        /// <param name = "data">The asynchronous data provider.</param>
        /// <param name = "flags">Flags specifying how the command should operate.</param>
        /// <returns>The data retrieved from the GPU.</returns>
        public DataStream GetData(Asynchronous data, AsynchronousFlags flags)
        {
            DataStream result = new DataStream(data.DataSize, true, true);
            GetDataInternal(data, result.DataPointer, (int) result.Length, flags);
            return result;
        }

        /// <summary>
        ///   Gets data from the GPU asynchronously.
        /// </summary>
        /// <param name = "data">The asynchronous data provider.</param>
        /// <param name = "flags">Flags specifying how the command should operate.</param>
        /// <returns>The data retrieved from the GPU.</returns>
        public T GetData<T>(Asynchronous data, AsynchronousFlags flags) where T : struct
        {
            unsafe
            {
                int size = Marshal.SizeOf(typeof (T));
                // TODO, verify if stackalloc is a good place to store async data
                byte* pBuffer = stackalloc byte[size];
                DataStream stream = new DataStream(pBuffer, size, true, true, false);

                GetDataInternal(data, stream.DataPointer, size, flags);

                return stream.Read<T>();
            }
        }

        /// <summary>	
        /// Copy the entire contents of the source resource to the destination resource using the GPU. 	
        /// </summary>	
        /// <remarks>	
        /// This method is unusual in that it causes the GPU to perform the copy operation (similar to a memcpy by the CPU). As a result, it has a few restrictions designed for improving performance. For instance, the source and destination resources:  Must be different resources. Must be the same type. Must have identical dimensions (including width, height, depth, and size as appropriate). Will only be copied. CopyResource does not support any stretch, color key, blend, or format conversions. Must have compatible DXGI formats, which means the formats must be identical or at least from the same type group. For example, a DXGI_FORMAT_R32G32B32_FLOAT texture can be copied to an DXGI_FORMAT_R32G32B32_UINT texture since both of these formats are in the DXGI_FORMAT_R32G32B32_TYPELESS group. Might not be currently mapped.  You cannot use an {{Immutable}} resource as a destination. You can use a   {{depth-stencil}} resource as either a source or a destination.  Resources created with multisampling capability (see <see cref="SharpDX.DXGI.SampleDescription"/>) can be used as source and destination only if both source and destination have identical multisampled count and quality. If source and destination differ in multisampled count and quality or if one is multisampled and the other is not multisampled the call to ID3D11DeviceContext::CopyResource fails. The method is an asynchronous call which may be added to the command-buffer queue. This attempts to remove pipeline stalls that may occur when copying data.  An application that only needs to copy a portion of the data in a resource should use <see cref="SharpDX.Direct3D11.DeviceContext.CopySubresourceRegion_"/> instead. 	
        /// </remarks>	
        /// <param name="source">A reference to the source resource (see <see cref="SharpDX.Direct3D11.Resource"/>). </param>
        /// <param name="destination">A reference to the destination resource (see <see cref="SharpDX.Direct3D11.Resource"/>). </param>
        /// <unmanaged>void ID3D11DeviceContext::CopyResource([In] ID3D11Resource* pDstResource,[In] ID3D11Resource* pSrcResource)</unmanaged>
        public void CopyResource(Resource source, Resource destination)
        {
            CopyResource_(destination, source);
        }

        /// <summary>	
        /// Copy a region from a source resource to a destination resource.	
        /// </summary>	
        /// <remarks>	
        /// The source box must be within the size of the source resource. The destination offsets, (x, y, and z) allow the source box to be offset when writing into the destination resource; however, the dimensions of the source box and the offsets must be within the size of the resource. If the resources are buffers, all coordinates are in bytes; if the resources are textures, all coordinates are in texels. {{D3D11CalcSubresource}} is a helper function for calculating subresource indexes. CopySubresourceRegion performs the copy on the GPU (similar to a memcpy by the CPU). As a consequence, the source and destination resources:  Must be different subresources (although they can be from the same resource). Must be the same type. Must have compatible DXGI formats (identical or from the same type group). For example, a DXGI_FORMAT_R32G32B32_FLOAT texture can be copied to an DXGI_FORMAT_R32G32B32_UINT texture since both of these formats are in the DXGI_FORMAT_R32G32B32_TYPELESS group. May not be currently mapped.  CopySubresourceRegion only supports copy; it does not support any stretch, color key, blend, or format conversions. An application that needs to copy an entire resource should use <see cref="SharpDX.Direct3D11.DeviceContext.CopyResource_"/> instead. CopySubresourceRegion is an asynchronous call which may be added to the command-buffer queue, this attempts to remove pipeline stalls that may occur when copying data. See performance considerations for more details. Note??If you use CopySubresourceRegion with a depth-stencil buffer or a multisampled resource, you must copy the whole subresource. In this situation, you must pass 0 to the DstX, DstY, and DstZ parameters and NULL to the pSrcBox parameter. In addition, source and destination resources, which are represented by the pSrcResource and pDstResource parameters, should have identical sample count values. Example The following code snippet copies a box (located at (120,100),(200,220)) from a source texture into a reqion (10,20),(90,140) in a destination texture. 	
        /// <code> D3D11_BOX sourceRegion;	
        /// sourceRegion.left = 120;	
        /// sourceRegion.right = 200;	
        /// sourceRegion.top = 100;	
        /// sourceRegion.bottom = 220;	
        /// sourceRegion.front = 0;	
        /// sourceRegion.back = 1; pd3dDeviceContext-&gt;CopySubresourceRegion( pDestTexture, 0, 10, 20, 0, pSourceTexture, 0, &amp;sourceRegion ); </code>	
        /// 	
        ///  Notice, that for a 2D texture, front and back are set to 0 and 1 respectively. 	
        /// </remarks>	
        /// <param name="source">A reference to the source resource (see <see cref="SharpDX.Direct3D11.Resource"/>). </param>
        /// <param name="sourceSubresource">Source subresource index. </param>
        /// <param name="sourceRegion">A reference to a 3D box (see <see cref="SharpDX.Direct3D11.ResourceRegion"/>) that defines the source subresources that can be copied. If NULL, the entire source subresource is copied. The box must fit within the source resource. </param>
        /// <param name="destination">A reference to the destination resource (see <see cref="SharpDX.Direct3D11.Resource"/>). </param>
        /// <param name="destinationSubResource">Destination subresource index. </param>
        /// <param name="dstX">The x-coordinate of the upper left corner of the destination region. </param>
        /// <param name="dstY">The y-coordinate of the upper left corner of the destination region. For a 1D subresource, this must be zero. </param>
        /// <param name="dstZ">The z-coordinate of the upper left corner of the destination region. For a 1D or 2D subresource, this must be zero. </param>
        /// <unmanaged>void ID3D11DeviceContext::CopySubresourceRegion([In] ID3D11Resource* pDstResource,[In] int DstSubresource,[In] int DstX,[In] int DstY,[In] int DstZ,[In] ID3D11Resource* pSrcResource,[In] int SrcSubresource,[In, Optional] const D3D11_BOX* pSrcBox)</unmanaged>
        public void CopySubresourceRegion(SharpDX.Direct3D11.Resource source, int sourceSubresource, SharpDX.Direct3D11.ResourceRegion? sourceRegion, SharpDX.Direct3D11.Resource destination, int destinationSubResource, int dstX, int dstY, int dstZ)
        {
            CopySubresourceRegion_(destination, destinationSubResource, dstX, dstY, dstZ, source, sourceSubresource, sourceRegion);
        }


        /// <summary>	
        /// Copy a multisampled resource into a non-multisampled resource.	
        /// </summary>	
        /// <remarks>	
        /// This API is most useful when re-using the resulting rendertarget of one render pass as an input to a second render pass. The source and destination resources must be the same resource type and have the same dimensions. In addition, they must have compatible formats. There are three scenarios for this:  ScenarioRequirements Source and destination are prestructured and typedBoth the source and destination must have identical formats and that format must be specified in the Format parameter. One resource is prestructured and typed and the other is prestructured and typelessThe typed resource must have a format that is compatible with the typeless resource (i.e. the typed resource is DXGI_FORMAT_R32_FLOAT and the typeless resource is DXGI_FORMAT_R32_TYPELESS). The format of the typed resource must be specified in the Format parameter. Source and destination are prestructured and typelessBoth the source and desintation must have the same typeless format (i.e. both must have DXGI_FORMAT_R32_TYPELESS), and the Format parameter must specify a format that is compatible with the source and destination (i.e. if both are DXGI_FORMAT_R32_TYPELESS then DXGI_FORMAT_R32_FLOAT could be specified in the Format parameter). For example, given the DXGI_FORMAT_R16G16B16A16_TYPELESS format:  The source (or dest) format could be DXGI_FORMAT_R16G16B16A16_UNORM The dest (or source) format could be DXGI_FORMAT_R16G16B16A16_FLOAT    ? 	
        /// </remarks>	
        /// <param name="source">Source resource. Must be multisampled. </param>
        /// <param name="sourceSubresource">&gt;The source subresource of the source resource. </param>
        /// <param name="destination">Destination resource. Must be a created with the <see cref="SharpDX.Direct3D11.ResourceUsage.Default"/> flag and be single-sampled. See <see cref="SharpDX.Direct3D11.Resource"/>. </param>
        /// <param name="destinationSubresource">A zero-based index, that identifies the destination subresource. Use {{D3D11CalcSubresource}} to calculate the index. </param>
        /// <param name="format">A <see cref="SharpDX.DXGI.Format"/> that indicates how the multisampled resource will be resolved to a single-sampled resource.  See remarks. </param>
        /// <unmanaged>void ID3D11DeviceContext::ResolveSubresource([In] ID3D11Resource* pDstResource,[In] int DstSubresource,[In] ID3D11Resource* pSrcResource,[In] int SrcSubresource,[In] DXGI_FORMAT Format)</unmanaged>
        public void ResolveSubresource(SharpDX.Direct3D11.Resource source, int sourceSubresource, SharpDX.Direct3D11.Resource destination, int destinationSubresource, SharpDX.DXGI.Format format)
        {
            ResolveSubresource_(destination, destinationSubresource, source, sourceSubresource, format);
        }

        /// <summary>
        ///   Maps a GPU resource into CPU-accessible memory.
        /// </summary>
        /// <param name = "resource">The resource to map.</param>
        /// <param name = "subresource">Index of the subresource to map.</param>
        /// <param name = "sizeInBytes">Size, in bytes, of the data to retrieve.</param>
        /// <param name = "mode">Specifies the CPU's read and write permissions for the resource. </param>
        /// <param name = "flags">Flags that specify what the CPU should do when the GPU is busy.</param>
        /// <returns>The mapped resource data.</returns>
        public DataBox MapSubresource(Resource resource, int subresource, MapMode mode, MapFlags flags)
        {
            unsafe
            {
                MappedSubResource mappedSubResource;
                Map(resource, subresource, mode, flags, out mappedSubResource);
                return new DataBox(mappedSubResource.RowPitch, mappedSubResource.DepthPitch,
                                   new DataStream((void*)mappedSubResource.PData, mappedSubResource.DepthPitch, true, true, false));
            }
        }

        /// <summary>
        ///   Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <param name = "source">The source data.</param>
        /// <param name = "resource">The destination resource.</param>
        /// <param name = "subresource">The destination subresource.</param>
        public void UpdateSubresource(DataBox source, Resource resource, int subresource)
        {
            UpdateSubresource(resource, subresource, null, source.Data.DataPointer, source.RowPitch, source.SlicePitch);
        }

        /// <summary>
        ///   Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <param name = "source">The source data.</param>
        /// <param name = "resource">The destination resource.</param>
        /// <param name = "subresource">The destination subresource.</param>
        /// <param name = "region">The destination region within the resource.</param>
        public void UpdateSubresource(DataBox source, Resource resource, int subresource, ResourceRegion region)
        {
            UpdateSubresource(resource, subresource, region, source.Data.DataPointer, source.RowPitch, source.SlicePitch);
        }
    }
}