package com.verstappen.memorypalace.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

public class ConceptDTO {

    @NotBlank(message = "Title cannot be empty")
    @Size(max = 255)
    private String title;

    @Size(max = 1000)
    private String description;

    @NotBlank(message = "Media URL cannot be empty")
    private String mediaUrl;

    private String memoryObject;
    private String location;
    private String visualCue;

    // GETTERS
    public String getTitle() {
        return title;
    }

    public String getDescription() {
        return description;
    }

    public String getMediaUrl() {
        return mediaUrl;
    }

    public String getMemoryObject() {
        return memoryObject;
    }

    public String getLocation() {
        return location;
    }

    public String getVisualCue() {
        return visualCue;
    }

    // SETTERS
    public void setTitle(String title) {
        this.title = title != null ? title.trim() : null;
    }

    public void setDescription(String description) {
        this.description = description != null ? description.trim() : null;
    }

    public void setMediaUrl(String mediaUrl) {
        this.mediaUrl = mediaUrl != null ? mediaUrl.trim() : null;
    }

    public void setMemoryObject(String memoryObject) {
        this.memoryObject = memoryObject != null ? memoryObject.trim() : null;
    }

    public void setLocation(String location) {
        this.location = location != null ? location.trim() : null;
    }

    public void setVisualCue(String visualCue) {
        this.visualCue = visualCue != null ? visualCue.trim() : null;
    }
}