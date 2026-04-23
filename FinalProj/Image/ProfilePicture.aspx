<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfilePicture.ascx.cs" Inherits="CanteenProject.Shared.ProfilePicture" %>
<style>
    .profile-container {
        text-align: center;
        padding: 0.5rem;
    }
    .profile-image-container {
        position: relative;
        display: inline-block;
        cursor: pointer;
    }
    .profile-img {
        width: 48px;
        height: 48px;
        border-radius: 50%;
        object-fit: cover;
        border: 2px solid #4f46e5;
        transition: transform 0.2s, box-shadow 0.2s;
        background: #1e293b;
    }
    .profile-img:hover {
        transform: scale(1.05);
        box-shadow: 0 0 15px rgba(79, 70, 229, 0.5);
    }
    .camera-icon {
        position: absolute;
        bottom: 0px;
        right: 0px;
        background: #4f46e5;
        border-radius: 50%;
        width: 20px;
        height: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 0.7rem;
        border: 2px solid #0f172a;
        cursor: pointer;
        transition: 0.2s;
    }
    .camera-icon:hover {
        background: #7c3aed;
        transform: scale(1.1);
    }
    .upload-modal {
        display: none;
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.8);
        z-index: 2000;
        justify-content: center;
        align-items: center;
    }
    .upload-modal-content {
        background: #1e293b;
        border-radius: 1rem;
        padding: 2rem;
        max-width: 400px;
        width: 90%;
        text-align: center;
    }
    .upload-modal-content h3 {
        color: #a78bfa;
        margin-bottom: 1rem;
    }
    .upload-preview {
        width: 150px;
        height: 150px;
        border-radius: 50%;
        object-fit: cover;
        margin: 1rem auto;
        border: 2px solid #4f46e5;
    }
    .file-input {
        margin: 1rem 0;
    }
    .file-input input {
        color: white;
    }
    .upload-buttons {
        display: flex;
        gap: 1rem;
        justify-content: center;
        margin-top: 1rem;
    }
    .btn-upload {
        background: #10b981;
        color: white;
        border: none;
        padding: 0.5rem 1rem;
        border-radius: 0.5rem;
        cursor: pointer;
    }
    .btn-cancel {
        background: #ef4444;
        color: white;
        border: none;
        padding: 0.5rem 1rem;
        border-radius: 0.5rem;
        cursor: pointer;
    }
    .msg-success { color: #34d399; font-size: 0.8rem; margin-top: 0.5rem; }
    .msg-error { color: #f87171; font-size: 0.8rem; margin-top: 0.5rem; }
</style>

<div class="profile-container">
    <div class="profile-image-container" onclick="openUploadModal()">
        <asp:Image ID="imgProfile" runat="server" CssClass="profile-img" 
            ImageUrl="~/Images/default-avatar.png" 
            AlternateText="Profile Picture" />
        <div class="camera-icon">📷</div>
    </div>
    <asp:Label ID="lblMessage" runat="server" CssClass="msg-success" Visible="false" />
</div>

<!-- Upload Modal -->
<div id="uploadModal" class="upload-modal">
    <div class="upload-modal-content">
        <h3>Upload Profile Picture</h3>
        <img id="previewImage" class="upload-preview" src="../Images/default-avatar.png" alt="Preview" />
        <div class="file-input">
            <asp:FileUpload ID="fileUpload" runat="server" Accept="image/jpeg,image/png,image/jpg,image/gif" />
        </div>
        <div class="upload-buttons">
            <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn-upload" OnClick="btnUpload_Click" />
            <button type="button" class="btn-cancel" onclick="closeUploadModal()">Cancel</button>
        </div>
        <asp:Label ID="lblUploadMessage" runat="server" CssClass="msg-error" Visible="false" />
    </div>
</div>

<script>
    function openUploadModal() {
        var modal = document.getElementById('uploadModal');
        if (modal) {
            modal.style.display = 'flex';
        }
    }

    function closeUploadModal() {
        var modal = document.getElementById('uploadModal');
        if (modal) {
            modal.style.display = 'none';
        }
        // Clear the file input and preview
        var fileInput = document.querySelector('#<%= fileUpload.ClientID %>');
        if (fileInput) {
            fileInput.value = '';
        }
        var preview = document.getElementById('previewImage');
        if (preview) {
            preview.src = '../Images/default-avatar.png';
        }
    }

    // Preview image before upload
    function setupPreview() {
        var fileInput = document.querySelector('#<%= fileUpload.ClientID %>');
        if (fileInput) {
            fileInput.addEventListener('change', function (e) {
                const reader = new FileReader();
                reader.onload = function (event) {
                    const preview = document.getElementById('previewImage');
                    if (preview) {
                        preview.src = event.target.result;
                    }
                }
                if (e.target.files[0]) {
                    reader.readAsDataURL(e.target.files[0]);
                }
            });
        }
    }

    // Close modal when clicking outside
    window.onclick = function (event) {
        const modal = document.getElementById('uploadModal');
        if (event.target === modal) {
            modal.style.display = 'none';
        }
    }

    // Initialize
    document.addEventListener('DOMContentLoaded', function () {
        setupPreview();
    });
</script>