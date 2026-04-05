// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Exceptions;

namespace DeckSurf.SDK.Tests.Exceptions
{
    public class ExceptionTests
    {
        // --- DeckSurfException ---

        [Fact]
        public void DeckSurfException_Parameterless_CanBeConstructed()
        {
            var ex = new DeckSurfException();

            Assert.NotNull(ex);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public void DeckSurfException_WithMessage_SetsMessage()
        {
            var ex = new DeckSurfException("test error");

            Assert.Equal("test error", ex.Message);
        }

        [Fact]
        public void DeckSurfException_WithMessageAndInner_PropagatesBoth()
        {
            var inner = new InvalidOperationException("inner");
            var ex = new DeckSurfException("outer", inner);

            Assert.Equal("outer", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void DeckSurfException_InheritsFromException()
        {
            var ex = new DeckSurfException();

            Assert.IsAssignableFrom<Exception>(ex);
        }

        // --- DeviceDisconnectedException ---

        [Fact]
        public void DeviceDisconnectedException_Parameterless_CanBeConstructed()
        {
            var ex = new DeviceDisconnectedException();

            Assert.NotNull(ex);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public void DeviceDisconnectedException_WithMessage_SetsMessage()
        {
            var ex = new DeviceDisconnectedException("device lost");

            Assert.Equal("device lost", ex.Message);
        }

        [Fact]
        public void DeviceDisconnectedException_WithMessageAndInner_PropagatesBoth()
        {
            var inner = new IOException("usb error");
            var ex = new DeviceDisconnectedException("device lost", inner);

            Assert.Equal("device lost", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void DeviceDisconnectedException_InheritsFromDeckSurfException()
        {
            var ex = new DeviceDisconnectedException();

            Assert.IsAssignableFrom<DeckSurfException>(ex);
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void DeviceDisconnectedException_DeviceSerial_CanBeSet()
        {
            var ex = new DeviceDisconnectedException("msg") { DeviceSerial = "ABC123" };

            Assert.Equal("ABC123", ex.DeviceSerial);
        }

        [Fact]
        public void DeviceDisconnectedException_DeviceSerial_DefaultsToNull()
        {
            var ex = new DeviceDisconnectedException();

            Assert.Null(ex.DeviceSerial);
        }

        [Fact]
        public void DeviceDisconnectedException_DeviceSerial_CanBeNull()
        {
            var ex = new DeviceDisconnectedException("msg") { DeviceSerial = null };

            Assert.Null(ex.DeviceSerial);
        }

        // --- DeviceCommunicationException ---

        [Fact]
        public void DeviceCommunicationException_Parameterless_CanBeConstructed()
        {
            var ex = new DeviceCommunicationException();

            Assert.NotNull(ex);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public void DeviceCommunicationException_WithMessage_SetsMessage()
        {
            var ex = new DeviceCommunicationException("io error");

            Assert.Equal("io error", ex.Message);
        }

        [Fact]
        public void DeviceCommunicationException_WithMessageAndInner_PropagatesBoth()
        {
            var inner = new IOException("stream closed");
            var ex = new DeviceCommunicationException("io error", inner);

            Assert.Equal("io error", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void DeviceCommunicationException_InheritsFromDeckSurfException()
        {
            var ex = new DeviceCommunicationException();

            Assert.IsAssignableFrom<DeckSurfException>(ex);
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void DeviceCommunicationException_IsTransient_True()
        {
            var ex = new DeviceCommunicationException("error") { IsTransient = true };

            Assert.True(ex.IsTransient);
        }

        [Fact]
        public void DeviceCommunicationException_IsTransient_False()
        {
            var ex = new DeviceCommunicationException("error") { IsTransient = false };

            Assert.False(ex.IsTransient);
        }

        [Fact]
        public void DeviceCommunicationException_IsTransient_DefaultsFalse()
        {
            var ex = new DeviceCommunicationException();

            Assert.False(ex.IsTransient);
        }

        // --- ImageProcessingException ---

        [Fact]
        public void ImageProcessingException_Parameterless_CanBeConstructed()
        {
            var ex = new ImageProcessingException();

            Assert.NotNull(ex);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public void ImageProcessingException_WithMessage_SetsMessage()
        {
            var ex = new ImageProcessingException("image decode failed");

            Assert.Equal("image decode failed", ex.Message);
        }

        [Fact]
        public void ImageProcessingException_WithMessageAndInner_PropagatesBoth()
        {
            var inner = new FormatException("bad format");
            var ex = new ImageProcessingException("image decode failed", inner);

            Assert.Equal("image decode failed", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void ImageProcessingException_InheritsFromDeckSurfException()
        {
            var ex = new ImageProcessingException();

            Assert.IsAssignableFrom<DeckSurfException>(ex);
            Assert.IsAssignableFrom<Exception>(ex);
        }

        // --- DeviceNotFoundException ---

        [Fact]
        public void DeviceNotFoundException_Parameterless_CanBeConstructed()
        {
            var ex = new DeviceNotFoundException();

            Assert.NotNull(ex);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public void DeviceNotFoundException_WithMessage_SetsMessage()
        {
            var ex = new DeviceNotFoundException("no device");

            Assert.Equal("no device", ex.Message);
        }

        [Fact]
        public void DeviceNotFoundException_WithMessageAndInner_PropagatesBoth()
        {
            var inner = new TimeoutException("timed out");
            var ex = new DeviceNotFoundException("no device", inner);

            Assert.Equal("no device", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void DeviceNotFoundException_InheritsFromDeckSurfException()
        {
            var ex = new DeviceNotFoundException();

            Assert.IsAssignableFrom<DeckSurfException>(ex);
            Assert.IsAssignableFrom<Exception>(ex);
        }
    }
}
