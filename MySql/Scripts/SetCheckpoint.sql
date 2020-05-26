IF EXISTS ( SELECT checkpoint_id FROM checkpoints WHERE checkpoint_id = @CheckpointId )
    UPDATE checkpoints
    SET checkpoint = @Checkpoint
    WHERE checkpoint_id = @CheckpointId;
ELSE
    INSERT INTO checkpoints (checkpoint_id, checkpoint)
    VALUES (@CheckpointId, @Checkpoint);