package com.verstappen.memorypalace.model;

import java.time.LocalDateTime;

import com.opencsv.bean.CsvBindByName;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.PrePersist;
import jakarta.persistence.PreUpdate;

@Entity
public class Concept {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @CsvBindByName(column = "title")
    private String title;

    @CsvBindByName(column = "description")
    @Column(length = 1000)
    private String description;

    @CsvBindByName(column = "mediaUrl")
    private String mediaUrl;

    @CsvBindByName(column = "memoryObject")
    private String memoryObject;

    @CsvBindByName(column = "location")
    private String location;

    @CsvBindByName(column = "visualCue")
    private String visualCue;

    // Review-state fields (added for frontend integration)
    @Column(nullable = false)
    private int strength = 0;

    @Column(nullable = false)
    private int repetitions = 0;

    private LocalDateTime lastReviewed;

    @Column(updatable = false)
    private LocalDateTime createdAt;

    private LocalDateTime updatedAt;

    @PrePersist
    protected void onCreate() {
        createdAt = LocalDateTime.now();
        updatedAt = LocalDateTime.now();
    }

    @PreUpdate
    protected void onUpdate() {
        updatedAt = LocalDateTime.now();
    }

    // GETTERS
    public Long getId() { return id; }
    public String getTitle() { return title; }
    public String getDescription() { return description; }
    public String getMediaUrl() { return mediaUrl; }
    public String getMemoryObject() { return memoryObject; }
    public String getLocation() { return location; }
    public String getVisualCue() { return visualCue; }
    public int getStrength() { return strength; }
    public int getRepetitions() { return repetitions; }
    public LocalDateTime getLastReviewed() { return lastReviewed; }
    public LocalDateTime getCreatedAt() { return createdAt; }
    public LocalDateTime getUpdatedAt() { return updatedAt; }

    // SETTERS
    public void setId(Long id) { this.id = id; }
    public void setTitle(String title) { this.title = title; }
    public void setDescription(String description) { this.description = description; }
    public void setMediaUrl(String mediaUrl) { this.mediaUrl = mediaUrl; }
    public void setMemoryObject(String memoryObject) { this.memoryObject = memoryObject; }
    public void setLocation(String location) { this.location = location; }
    public void setVisualCue(String visualCue) { this.visualCue = visualCue; }
    public void setStrength(int strength) { this.strength = strength; }
    public void setRepetitions(int repetitions) { this.repetitions = repetitions; }
    public void setLastReviewed(LocalDateTime lastReviewed) { this.lastReviewed = lastReviewed; }
    public void setCreatedAt(LocalDateTime createdAt) { this.createdAt = createdAt; }
    public void setUpdatedAt(LocalDateTime updatedAt) { this.updatedAt = updatedAt; }
}
